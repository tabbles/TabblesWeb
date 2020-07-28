

async function doPost(url, inp) {
    console.log(`facendo post a ${url}`, inp);
    return $.ajax({ type: "POST", url: url, data: JSON.stringify(inp), contentType: "application/json" });
}

async function doGet(url) {
    console.log(`facendo get a ${url}`);
    return $.ajax({ type: "GET", url: url, contentType: "application/json" });
}

var getFilenameOfPath = function (path) {
    if (path === null) {
        debugger;
    }
    return path.split('\\').pop().split('/').pop();
};


function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var binaryLen = binaryString.length;
    var bytes = new Uint8Array(binaryLen);
    for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
    }
    return bytes;
}


function saveByteArray(reportName, byte) {
    var blob = new Blob([byte], { type: "application/pdf" });
    var link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    var fileName = reportName;
    link.download = fileName;
    link.click();
}



if (!String.prototype.endsWith)
{
	String.prototype.endsWith = function (search, this_len)
	{
		if (this_len === undefined || this_len > this.length)
		{
			this_len = this.length;
		}
		return this.substring(this_len - search.length, this_len) === search;
	};
}