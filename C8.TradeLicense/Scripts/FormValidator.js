function SetDropdowns(discriminator) {
    //var $ = jQuery.noConflict();

    $.getJSON("/Common/GetDropDown?discriminator=" + discriminator, function (result) {

        if (result != "Error" && result != "[]") {
            var dropDownValues = result;//JSON.parse(result);

            if (dropDownValues.Discriminator == discriminator) {

                //$("#LicenceHolderz_ApplicationType").empty();
                //$("#LicenseType").empty();
                //$("#DocumentType").empty();

                //$("#TypeOfBusinessConducted").empty();
                //$("#LicenceHolderz_TypeOfBusinessConducted").empty();
                var tps = dropDownValues.values;
                $.each(tps, function (i, Value) {
                    var deletee = Value.Value;
                    var controlID = "";
                    if (Value.Discriminator == "BusinessType") {
                        controlID = "LicenceHolderz_BusinessType";
                        // $("#LicenceHolderz_TypeOfBusiness").append('<option value="' + Value.Text + '">' + Value.Text + '</option>');
                    }
                    else if (Value.Discriminator == "LicenseType") {
                        controlID = "LicenceHolderz_ApplicationType";

                        //$("#ApplicationType").append('<option value="' + Value.Text + '">' + Value.Text + '</option>');
                    }
                    else if (Value.Discriminator == "TypeOfBusinessConducted") {
                        controlID = "LicenceHolderz_TypeOfBusinessConducted";
                        //$("#LicenceHolderz_TypeOfBusinessConducted").append('<option value="' + Value.Text + '">' + Value.Text + '</option>');
                    }
                    else if (Value.Discriminator == "ReviewOutcome") {
                        controlID = "ReviewOutcome";
                        //$("#ReviewOutcome").append('<option value="' + Value.Text + '">' + Value.Text + '</option>');
                    } else if (Value.Discriminator == "SameAsApplicant") {
                        controlID = "SameAsApplicant";                      
                    }
                    else if (Value.Discriminator == "InspectionOutcome") {
                        controlID = "InspectionOutcome";                      
                    } else if (Value.Discriminator == "RecommendationOutcome") {
                        controlID = "RecommendationOutcome";
                    }                    
                    else {
                        controlID = "DocumentType";                      
                    }
                    $("#" + controlID).append('<option value="' + Value.Text + '">' + Value.Text + '</option>');
                })
                SetDropDownSelectedValues();
            }

        }
        else {
            if ("@ViewBag.ErrorMessage" == "") {

                swal("", "Erorr Occured retrieving details", "Error");
            }

        }
    });

}
function clearChildren(element) {
    for (var i = 0; i < element.childNodes.length; i++) {
        var e = element.childNodes[i];
        if (e.tagName) switch (e.tagName.toLowerCase()) {
            case 'input':
                e.required = "";
                switch (e.type) {
                    case "radio":
                    case "checkbox": e.checked = false; break;
                    case "button":
                    case "submit":
                    case "image": break;
                    default: e.value = ''; break;
                }
                break;
            case 'select': e.selectedIndex = 0;
                e.required = "";
                break;
            case 'textarea': e.value = '';
                e.required = "";
                break;
            default: clearChildren(e);
        }
    }
}
function checkEmail(email) {

    var txt = email.value;
    var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

    if (!filter.test(email.value)) {
        email.style.border = '1px solid red';
    }
    else {
        email.style.border = '';
    }

    if (txt == "")
        email.style.border = '';
}
function ValidateIDNumber(IDNum) {
    var txt = IDNum.value;
    if (txt.length != 13 && txt != "") {
        redborder(IDNum);
    }
    else {
        IDNum.style.border = '';
    }
}
function ValidateCellNumber(CellNum) {
    var txt = CellNum.value;
    if (txt.length != 10 && txt.length > 0) {
        redborder(CellNum);
    }
    else {
        CellNum.style.border = '';
    }
}
var className = "";
function redborder(control) {
    className = control.className;
    control.className = "md-input md-input-danger";
    control.style.border = '1px solid red';
    return false;
}
function RemoveRedBorder(control) {
    control.className = className;
    if (control.value != "") {
        control.style.border = '';
    }
}
function allowNumeric(e) {

    var code = ('charCode' in e) ? e.charCode : e.keyCode;
    if (!(code > 47 && code < 58)) // numeric (0-9)
    {
        e.preventDefault();
    }
}
function ValidateZipCode(ZipCode) {

    var txt = ZipCode.value;

    if (txt.length != 4 && txt.length > 0) {

        ZipCode.style.border = '1px solid red';
    }
    else {
        ZipCode.style.border = '';
    }

}
function allowAlphaNumericSpace(e) {
    var code = ('charCode' in e) ? e.charCode : e.keyCode;
    if (!(code == 32) && // space

      !(code > 64 && code < 91) && // upper alpha (A-Z)

      !(code > 96 && code < 123)) { // lower alpha (a-z)
        e.preventDefault();
    }
}
function GetDocumentList(isNew) {

    // //initLoadHoldOnClose();('Retrieving Documents....Please wait');
    var appID = 0;
    //var $ = jQuery.noConflict();
    if (isNew != "isNew") {
        $.getJSON("/Common/GetDocumentList?FolderName=" + appID,
         function (result) {
             //initLoadHoldOnClose();
             if (result != "" && result != "[]" && result != "Error") {
                 debugger
                 PopulateDocumentTable(result);
             }
         })
    }

} function PopulateDocumentTable(SearchResults) {
    for (var i = 0; i < SearchResults.response.length; i++)
    {
        var row = $("<tr />")
        $("#tblDocList").append(row);
        row.append($("<td>" + SearchResults.response[i].CREATION_DATE + "</td>"));
        if (SearchResults.response[i].REFERENCE_NUMBER != null) {
            row.append($("<td>" + SearchResults.response[i].REFERENCE_NUMBER + "</td>"));
        }
        else {
            row.append($("<td>" + "" + "</td>"));
        }
        var k = "<td> <a href=" + SearchResults.response[i].Link_URL + " target=\"_blank\" >" + SearchResults.response[i].NAME + "</a></td>";
        row.append($(k));
    }
    $('#tblDocList').DataTable().destroy();
    $('#tblDocList').DataTable({
        "searching": false,
        "paging": false,
        "ordering": false,
        "info": false
    });
}


var fileExtensions = ".doc,.dot,.docx,.docm,dotx,.xlt,.xls,.pdf.txt,.tif,.tiff,.gif,.jpeg,jpg,.jif,.jfif,.jp2,.jpx,.j2k, .j2c,.fpx,.pcd,.png";
function fileSelected() {

    try { $('#reqResponse').hide(); } catch (cat) { }
    try { $('#reqResponse2').hide(); } catch (cat) { }
    var id = "";
    var reqResponseLable = "";

    file = document.getElementById('fileToUpload').files[0];

    var ext = "";
    var re = /(?:\.([^.]+))?$/;
    if (file) {
        ext = "." + re.exec(file.name)[1];
    }
    // var getExtension
    if (fileExtensions.indexOf(ext.toLowerCase()) > -1) {
        if (file) {

            //var fileSize = 0;
            //if (file.size > 1024 * 1024)
            //    fileSize = (Math.round(file.size * 100 / (1024 * 1024)) / 100).toString() + 'MB';
            //else
            //    fileSize = (Math.round(file.size * 100 / 1024) / 100).toString() + 'KB';                        
        }
    }
    else {
        if (ext != "") {

            swal("", "file not supported", "Error");
        }
    }
}

function uploadFile() {
    document.getElementById("uploadDocument").disabled = true;
    fd = new FormData();
    var xhr = new XMLHttpRequest();
    var fileDescriptionComment = "";
    var fileType = "";
    try { fileType = document.getElementById("DocumentType").value; } catch (cat) { }


    fd.append("fileToUpload", document.getElementById('fileToUpload').files[0]);

    initLoadHoldOn("Uploading Document....Please wait");
    xhr.upload.addEventListener("progress", uploadProgress, false);
    xhr.addEventListener("load", uploadComplete, false);
    xhr.addEventListener("error", uploadFailed, false);
    xhr.addEventListener("abort", uploadCanceled, false);

    xhr.open("POST", "../../Common/UploadDocument?fileName=" + "" + "&fileType=" + fileType + "&fileDescriptionComment=" + fileDescriptionComment)
    xhr.send(fd);
}

function uploadProgress(evt) {
    if (evt.lengthComputable) {
        var percentComplete = Math.round(evt.loaded * 100 / evt.total);
        // document.getElementById('progressNumber').innerHTML = percentComplete.toString() + '%';
    }
    else {
        // document.getElementById('progressNumber').innerHTML = 'unable to compute';
    }
}
function uploadComplete(evt) {
    document.getElementById("uploadDocument").disabled = false;
    initLoadHoldOnClose();
    // document.getElementById('processing').className = "";

    /* This event is raised when the server send back a response */
    debugger;
    var res = evt.target.response;

    if (res != "\"Error\"" && res.indexOf("Error") < 0)//unsuccessful
    {
        document.getElementById('fileToUpload').value = "";
        swal("", "upload Successful!", "success");

        /////
        var jR = JSON.parse(res);
        if (jR.createdBy != "") {
            var fileName = jR.name;
            var link = jR.link_url;
            var dateUploaded = jR.creation_Date.replace("T", " ");
            var filetype = jR.reference_Number;
            var AuditDisplay = "";
            dateUploaded = dateUploaded.replace("Z", "");
            ///////////////////////////
            var row = $("<tr />")

            $("#tblDocList").append(row);
            row.append($("<td>" + dateUploaded + "</td>"));
            row.append($("<td>" + filetype + "</td>"));
         
            var k = "<td> <a href=" + link + " target=\"_blank\" >" + fileName + "</a></td>";

            row.append($(k));
            //var chk = "<td><input id='" + fileName + "'value='" + fileName + "'  name=\'chk\' type=\'checkbox\' onclick=SelectedItem(this)></td>"
            //AuditDisplay = AuditDisplay + chk;
            row.append($(chk));
            $('#tblDocList').DataTable().destroy();
            $('#tblDocList').DataTable();


        }
    }
    else {
        ErrorUpload();
    }
}
function ErrorUpload() {
    document.getElementById("uploadDocument").disabled = false;
    initLoadHoldOnClose();

    try { $('#reqResponse').show(); $('#reqResponse').text("File upload failed!"); } catch (cat) { }
}
function uploadFailed(evt) {
    ErrorUpload();
    initLoadHoldOnClose();
}

function uploadCanceled(evt) {
    initLoadHoldOnClose();

    swal("", "The upload has been canceled by the user or the browser dropped the connection.", "Error");
}