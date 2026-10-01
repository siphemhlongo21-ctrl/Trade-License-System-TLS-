using C8.TradeLicense.DataAccessLayer;
using C8.TradeLicense.Helpers;
using C8.TradeLicense.Keys;
using C8.TradeLicense.Models;
using C8.TradeLicense.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace C8.TradeLicense.Controllers
{
    public class AmendmentController : Controller
    {
        private readonly TradeLicenseDbContext _context;

        public AmendmentController()
        {
            _context = new TradeLicenseDbContext();
            IdentityManager = new IdentityManager(_context);
        }
        public IdentityManager IdentityManager { get; set; }
        /// <summary>
        /// Gets or sets the SystemUser.
        /// </summary>
        public User Users { get; set; }


        public int UserId { get; set; }

        /// <summary>
        /// The Initialise.
        /// </summary>
        private void Initialise()
        {

            try
            {
                IdentityManager = new IdentityManager(_context);

                if (User != null && User.Identity.IsAuthenticated)
                {
                    IdentityManager.CurrentUser(User);
                    Users = IdentityManager.CurrentUser(User);
                }

                if (Users != null)
                {
                    Users =
                        _context.Users.Where(o => o.UserId == Users.UserId)
                            .FirstOrDefault();
                }


            }
            catch (Exception ex)
            {
                throw;
            }

        }

        // GET: Amendment
        [Authorize(Roles = "Licensing Administrator" + "," + "Licensing Clerk" + "," + "Licensing Manager" + "," + "System Admin" + "," + "Chief Inspector")]
        public ActionResult Index()
        {
            Status adminReviewStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).FirstOrDefault();
            Status chiefReviewStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.AwaitingChiefReview).FirstOrDefault();
            Status managerReviewStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerReview).FirstOrDefault();
            Status amendmentRequestStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.LicenseAmendmentRequested).FirstOrDefault();

            int statusId = amendmentRequestStatus.StatusId;
            if (User.IsInRole(RoleKeys.LicensingAdministrator) || amendmentRequestStatus.StatusKey == StatusKeys.LicenseAmendmentRequested)
            {
                statusId = amendmentRequestStatus.StatusId;
            }
            else if (User.IsInRole(RoleKeys.ChiefInspector))
            {
                statusId = chiefReviewStatus.StatusId;
            }
            else if (User.IsInRole(RoleKeys.LicensingManager))
            {
                statusId = managerReviewStatus.StatusId;
            }

            var model = _context.LicenseAmendments.Include(l => l.License)
            .Where(x => x.StatusId == statusId)
            .Select(x => new AmendmentListVM
            {
                AmendmentId = x.AmendmentId,
                LicenseId = x.LicenseId,
                LicenseNumber = x.License.LicenseNumber,
                AmendmentType = x.AmendmentType,
                RequestedDate = x.RequestedDate,
                Status = "Pending Review"
            })
            .ToList();


            return View(model);
        }

        [Authorize(Roles = "Licensing Administrator" + "," + "Chief Inspector" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Review(int id)
        {
            var amendment = _context.LicenseAmendments.Include(l => l.License)
                .FirstOrDefault(x => x.AmendmentId == id);

            var ignoredFields = new[]
            {
            "CreatedDateTime",
            "ModifiedDateTime",
            "IsActive",
            "IsDeleted",
            "IsLocked",
            "CreatedBySystemUserId",
            "ModifiedBySystemUserId",
            "ApplicationDateTime",
            "LicenseIssueDateTime",
            "LicenseExpiryDate",
            "StatusId"
            };

            var changes = _context.Amendments
                .Where(x =>
                    x.LicenseId == amendment.LicenseId &&
                    !ignoredFields.Contains(x.FieldAmended))
                .Select(x => new FieldChangeVM
                {
                    TableName = x.TableAmended,
                    FieldName = x.FieldAmended,
                    OldValue = x.OldData,
                    NewValue = x.NewData
                })
                .ToList();

            var model = new AmendmentReviewVM
            {
                AmendmentId = amendment.AmendmentId,
                LicenseId = amendment.LicenseId,
                LicenseNumber = amendment.License.LicenseNumber,
                AmendmentType = amendment.AmendmentType,
                Changes = changes,
                BusinessDocuments = _context.FileUploads.Include(d=>d.Document).Where(f => f.referenceId == amendment.AmendmentId).ToList()
            };
            model.ApprovalDisplay = User.IsInRole(RoleKeys.LicensingAdministrator) ? "Recommend for Chief Review" :
                User.IsInRole(RoleKeys.ChiefInspector) ? "Recommend for Manager Review" :
                "Approve Amendment";
            model.RejectionDisplay = User.IsInRole(RoleKeys.LicensingAdministrator) ? "Reject Amendment" :
                User.IsInRole(RoleKeys.ChiefInspector) ? "Reject Amendment" :
                "Reject Amendment";
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Licensing Administrator" + "," + "Chief Inspector" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Approve(int amendmentId)
        {
            Initialise();
            var amendment = _context.LicenseAmendments
                .FirstOrDefault(x => x.AmendmentId == amendmentId);

            Status adminReviewStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.AwaitingAdministratorReview).FirstOrDefault();
            Status chiefReviewStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.AwaitingChiefReview).FirstOrDefault();
            Status managerReviewStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.AwaitingManagerReview).FirstOrDefault();
            Status amendmentApprovedStatus = _context.Status.Where(s => s.StatusKey == StatusKeys.LicenseAmendmentApproved).FirstOrDefault();

            if (amendment.StatusId == adminReviewStatus.StatusId)
            {
                amendment.StatusId = chiefReviewStatus.StatusId;

                AdministratorReview adminReview = new AdministratorReview
                {
                    DateReviewed = DateTime.Now,
                    Comment = "Recommended By Licensing Administrator",
                    Decision = "Recommend",
                    UserId = Users.UserId,
                    LicenseId = amendment.LicenseId
                };
                _context.AdministratorReviews.Add(adminReview);
                _context.LicenseAmendments.AddOrUpdate(amendment);
                _context.SaveChanges();

                //Notify Chief Inspector of pending review
                new LicenseHelper().SendLicenseForChiefReview(amendmentId, this);
            }
            else if (amendment.StatusId == chiefReviewStatus.StatusId)
            {
                amendment.StatusId = managerReviewStatus.StatusId;

                ChiefReview chiefReview = new ChiefReview
                {
                    DateReviewed = DateTime.Now,
                    Comment = "Recommended By Licensing Chief Inspector",
                    Decision = "Recommend",
                    UserId = Users.UserId,
                    LicenseId = amendment.LicenseId
                };
                _context.ChiefReview.Add(chiefReview);
                _context.LicenseAmendments.AddOrUpdate(amendment);
                _context.SaveChanges();

                //Notify Licensing Manager of pending review
                new LicenseHelper().SendLicenseForManagerReview(amendmentId, this);
            }
            else
            {
                amendment.StatusId = amendmentApprovedStatus.StatusId;
                amendment.ApprovedDate = DateTime.Now;
                _context.LicenseAmendments.AddOrUpdate(amendment);
                _context.SaveChanges();

                //TODO: Apply the changes to the license record
                new LicenseHelper().AmendmentApproved(amendment.AmendmentId);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Licensing Administrator" + "," + "Chief Inspector" + "," + "Licensing Manager" + "," + "System Admin")]
        public ActionResult Reject(int amendmentId, string reason)
        {
            var amendment = _context.LicenseAmendments
                .FirstOrDefault(x => x.AmendmentId == amendmentId);

            amendment.StatusId = 1053;
            amendment.RejectedDate = DateTime.Now;
            amendment.RejectionReason = reason;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}