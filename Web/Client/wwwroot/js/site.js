function HideModal(id) {
    $('#' + id).modal('hide')
}

function ShowModal(id) {
    $('#' + id).modal('show')
}

$('#datePicker').pickdate();

//function getFileData(inputId) {
//    var file = document.getElementById(inputId).files[0];
//    if (file) {
//        // create reader
//        var reader = new FileReader();
//        reader.readAsText(file);
//        reader.onload = function (e) {
//            var data = {
//                content: e.result.split(',')[1]
//            };  
//            return data;
//        };
//    }
//}

//// Resource: https://remibou.github.io/Upload-file-with-Blazor/
//const readUploadedFileAsText = (inputFile) => {
//    const temporaryFileReader = new FileReader();
//    return new Promise((resolve, reject) => {
//        temporaryFileReader.onerror = () => {
//            temporaryFileReader.abort();
//            reject(new DOMException("Problem parsing input file."));
//        };
//        temporaryFileReader.addEventListener("load", function () {
//            var data = {
//                content: temporaryFileReader.result.split(',')[1]
//            };
//            resolve(data);
//        }, false);
//        temporaryFileReader.readAsDataURL(inputFile.files[0]);
//    });
//};


//window.getFileData = (inputFile) => {
//    var uploadFile = document.getElementById(inputFile);
//    return readUploadedFileAsText(uploadFile);
//};
//Blazor.registerFunction("getFileData", function (inputFile) {
    
//});


