
let documnetHR = '';
let documnetFID = '';
let signature = '';

function addSpecimentOnline(nofid, jenis, flag) {
    
    var document = $.ajax({
        url: URL_PTPR + '/OnlineForm/SpecimenDoc?nofid=' + nofid,
        type: 'GET',
        dataType: "json",
        cache: false,
        async: false,
        contentType: false,
        processData: false,
        success: function (data) {
            debugger

            if (data != null) {

                signature = data.signature.Data.img64string;

                for (var i = 0; i < data.data.length; i++) {
                    debugger
                    var category = data.data[i].Category;

                    if (category == "HR") {
                        var imgSepcimen = "data:image/png;base64," + signature;
                        documnetHR = data.data[i].HtmlTag;

                        var modHtml = insertImageIntoTD(documnetHR, imgSepcimen, jenis, flag, category);

                        documnetHR = '';
                        documnetHR = modHtml;
                    }
                    else {
                        var imgSepcimen = "data:image/png;base64," + signature;
                        documnetFID = data.data[i].HtmlTag;

                        var modHtml = insertImageIntoTD(documnetFID, imgSepcimen, jenis, flag, category);
                        documnetFID = '';
                        documnetFID = modHtml;
                    }

                }

            }
            else {
                w2alert("Data Is Null");
            }

            clear_wait();
        }
    });
}

function insertImageIntoTD(htmlString, img64string, jenis, flag, category) {
    debugger
    // Create a temporary div element to hold the HTML content
    var tempDiv = document.createElement('div');
    tempDiv.innerHTML = htmlString;
    var spanElement = document.createElement('span');
    var currentDate = new Date();
    var options = { year: 'numeric', month: 'long', day: '2-digit' };
    spanElement.textContent = 'Tanggal: ' + currentDate.toLocaleDateString('id-ID', options);

    // Create a new <img> element
    var imgElement = document.createElement('img');

    if (jenis == 'BD') {
        if (flag == 'MANBPD') {
            if (category == 'FID') {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-1');
                tdElement.textContent = '';
                var tdTangaal = tempDiv.querySelector('#date-sgtr-1');
                tdTangaal.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen');
                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);
                tdTangaal.appendChild(spanElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
        else if (flag == "MANFBS") {
            if (category == "FID") {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-3');
                tdElement.textContent = '';
                var tdTangaal = tempDiv.querySelector('#date-sgtr-3');
                tdTangaal.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen-small');

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);
                tdTangaal.appendChild(spanElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
        else if (flag == "MANCNL") {
            if (category == "FID") {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-4');
                tdElement.textContent = '';
                var tdTangaal = tempDiv.querySelector('#date-sgtr-4');
                tdTangaal.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen-small');
                // Resize image hereeeeee

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);
                tdTangaal.appendChild(spanElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
        else if (flag == "MANCSPRM") {
            if (category == "FID") {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-2');
                tdElement.textContent = '';
                var tdTangaal = tempDiv.querySelector('#date-sgtr-2');
                tdTangaal.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen-small');
                // Resize image hereeeeee

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);
                tdTangaal.appendChild(spanElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
            else if (category == 'HR') {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#specimen-hr');
                tdElement.textContent = '';
                var tdTangaal = tempDiv.querySelector('#date-hr-sgtr');
                tdTangaal.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen');

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);
                tdTangaal.appendChild(spanElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
    }
    else {
        if (flag == 'MANCSPRM') {
            if (category == "FID") {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-2');
                tdElement.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen-small');
                // Resize image hereeeeee

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
        if (flag == 'MANFBS') {
            if (category == "FID") {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-3');
                tdElement.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen-small');
                // Resize image hereeeeee

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
        if (flag == 'MANCNL') {
            if (category == "FID") {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-4');
                tdElement.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen-small');
                // Resize image hereeeeee

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
        if (flag == 'MANBPD') {
            if (category == 'HR') {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#specimen-hr');
                tdElement.textContent = '';
                var tdTangaal = tempDiv.querySelector('#date-hr-sgtr');
                tdTangaal.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen');

                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);
                tdTangaal.appendChild(spanElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
            else if (category == 'FID') {
                // Get the <td> element by its ID
                var tdElement = tempDiv.querySelector('#ttd-fid-1');
                tdElement.textContent = '';
                var tdTangaal = tempDiv.querySelector('#date-sgtr-1');
                tdTangaal.textContent = '';

                // Set the src attribute to the base64 string
                imgElement.src = img64string;
                imgElement.classList.add('img-specimen-small');
                // Append the <img> element to the <td> element
                tdElement.appendChild(imgElement);
                tdTangaal.appendChild(spanElement);

                // Return the modified HTML content as a string
                return tempDiv.innerHTML;
            }
        }
    }


}

