var uname ;
var pwd ;
var gUserData;




var gOpenTabbles = [];
var gSuggested = [];
var gIsTaggedIsOpen = false;

var gTemplFile;
var gTemplateAtbTabble;
var gTemplateAtbPlus;

var gBrowser;
var gTabblesWebRows;

//function toDic(arr) {
//    let ret = {};
//    arr.forEach(i => {
//        debugger;
//        var key = i.Key;
//        var va = i.Value;
//        ret[key] = va;
//    });
//    return ret;
//}

function hashTabble(ta) {
    if (ta.pgName !== undefined) {
        return `phtag${ta.pgName}§${ta.pgIdOwner}`;
    }

    throw "non gestito";
}


function rebuildAtb() {
    $(".templatePlusAtb").remove();
    $(".templateAtbTabble").remove();

    if (gIsTaggedIsOpen) {
        let te = gTemplateAtbTabble.clone();
        //debugger;
        te.find(".spanTagName").text("Is Tagged");


        te.find(".fa-square.icona").css("color", "#444444");




        te.find(".btnTagClose").click(async e => {
            gIsTaggedIsOpen = false;
            rebuildAtb();

            rebuildFilePanel();
        });

        $(".filePanelAddressBar").append(te);


        //if (i < gOpenTabbles.length - 1) {
        //    let teplus = gTemplateAtbPlus.clone();
        //    $(".filePanelAddressBar").append(teplus);
        //}
    }

    for (i = 0; i < gOpenTabbles.length; i++) {
        var curTa = gOpenTabbles[i];

        let te = gTemplateAtbTabble.clone();
        //debugger;
        te.find(".spanTagName").text(curTa.tagName);


        te.find(".fa-circle.icona").css("color", curTa.colorStr);




        te.find(".btnTagClose").click(async e => {
            gOpenTabbles = gOpenTabbles.filter(x => x !== curTa);

            forseAggiungiIsTagged();

            rebuildAtb();

            rebuildFilePanel();
        });

        $(".filePanelAddressBar").append(te);


        if (i < gOpenTabbles.length - 1) {
            let teplus = gTemplateAtbPlus.clone();
            $(".filePanelAddressBar").append(teplus);
        }
    }

}


function forseAggiungiIsTagged() {


    let searchFilter = $("#txtSearch").val();
    // if is tagged must be open automatically, do it

    if (gOpenTabbles.length === 0 && searchFilter !== "") {
        gIsTaggedIsOpen = true;

        rebuildAtb();
    }


}

// legge gOpenTabbles
async function rebuildFilePanel() {


    let searchFilter = $("#txtSearch").val();


    let inp = {
        uname: uname,
        pwd: pwd,
        machineName: gUserData.machineName,
        ltags: gOpenTabbles,
        ctsFromFolder: [] /* todo il web server dovrebbe dirmi che file ci sono nella cartella che sto guardando, quando avremo le cartelle */,
        
        lnegtags: [],
        negExts: [],
        txtSearch: searchFilter,
        

         workspaceTagName: gUserData.wsTagName,
        workspaceTagIdOwner: gUserData.uid /*gUserData.wsTagId sbagliato! qui devo passare l'id owner, non l'id tag*/
        , isTagged: gIsTaggedIsOpen

    };
    let url = `${prefissoWebApi}/api/computeSuggestedTabblesAndCtsOfOpenTabbles`;
    let ret = await doPost(url, inp);


    console.log("risultato: ", ret);
    
    let files = ret.ret.dataTables[0];

    gSuggested = ret.ret.obj.stcAllSuggestedNodes2;
    console.log("suggested", gSuggested);

        let xxx = ret.ret.obj;
        let datatables = ret.ret.dataTables;
        // i commenti sono la quarta tabella [4],ma devo indicizzarli
        let tabComments = datatables[4];
        let dicComments = _.indexBy(tabComments, x => x.idTaggable);
    //debugger;

    $(".repFiles .templateFile").remove();

    

    files.forEach(fi => {

        if (fi.fiPath !== null) { // could be a url or email
            let lettera = fi.fiPath.substring(0, 1).toLowerCase();
            let rigaConQuellaLettera = _(gTabblesWebRows).find(ro => ro.letter.toLowerCase() === lettera);

    

            let te = gTemplFile.clone();


            let allowDownload = false;
            if (typeof rigaConQuellaLettera !== 'undefined') {
                //debugger;
                allowDownload = rigaConQuellaLettera.allowDowload;

            
            }


            if (!allowDownload) {
                te.addClass("nodownload");
            }
            let pathOrig = fi.fiPath;
        
            let fname = getFilenameOfPath(fi.fiPath);


            // faccio traduzione del fi.fiPath 
            {
                
                if (typeof rigaConQuellaLettera !== 'undefined') {
                    if (gBrowser.toLowerCase().startsWith("ios")
                        || gBrowser.toLowerCase().startsWith("iphone")
                        //|| gBrowser.toLowerCase().startsWith("mac os")
                        || gBrowser.toLowerCase().startsWith("ipad")) {


                        let prefisso = rigaConQuellaLettera.convertIos;
                        fi.fiPath = prefisso + fi.fiPath.substring(3); // salto C:\

                        fi.fiPath = fi.fiPath.replace(/\\/g, "/");
                    }
                    else if (gBrowser.toLowerCase().startsWith("mac os")) {


                        let prefisso = rigaConQuellaLettera.convertMac;
                        fi.fiPath = prefisso + fi.fiPath.substring(3); // salto C:\

                        fi.fiPath = fi.fiPath.replace(/\\/g, "/");
                    }
                    else if (gBrowser.toLowerCase().startsWith("android")) {


                        let prefisso = rigaConQuellaLettera.convertAndroid;
                        fi.fiPath = prefisso + fi.fiPath.substring(3); // salto C:\

                        fi.fiPath = fi.fiPath.replace(/\\/g, "/");
                    }

                }
            }

            //console.log("prima di replace", fi.fiPath);
            

            //console.log("dopo di replace", fi.fiPath);
            
            let path = fi.fiPath.replace(fname, '');
            te.find(".filePath").text(path);
                te.find(".fileName").text(fname);


                // commento sotto al file (mostra ultimo commento, troncato)
                
                if (dicComments.hasOwnProperty(fi.idCt))
                {
                        let commento = dicComments[fi.idCt];
                        let testoCommentoTrunc = commento.commentTextTrunc;
                        
                        //debugger;
                        let divcommento = te.find('.testoCommento');
                        divcommento.text(testoCommentoTrunc);
                        te.find('.autoreCommento').text(commento.userName);

                        te.find('.commentOuter').show();
                }
                else
                {
                        te.find('.commentOuter').hide();
                }

            te.find(".txtFullPathTranslatedForDevice").val(fi.fiPath);

            //te.find(".txtFullPathTranslatedForDevice").click(e => {
            //    //e.preventDefault();
            //    e.stopPropagation(); // non scaricare il file
            //    te.find(".txtFullPathTranslatedForDevice").select();

            //    console.log("selezionando tutto.");
            //});


            te.find(".btnCopyToClip").click(e => {
                e.preventDefault();
                e.stopPropagation();
                let copyText = "ciao";
                

                te.find(".txtFullPathTranslatedForDevice").select();
                document.execCommand("copy");
            });

            if (allowDownload) {
                te.find('.btnDownload').click(async function (e) {


                    // scarica

                    if (te.hasClass("disabled")) {
                        return;
                    }

                    console.log("cliccato file. scaricando.");

                    te.addClass("disabled");
                    let inp = {
                        uname: uname,
                        pwd: pwd,
                        path: pathOrig
                    };
                    let url = `${prefissoWebApi}/api/downloadFile`;
                    let ret = await doPost(url, inp);

                    if (ret.error === "file-not-found") {
                        alert("file not found");
                        te.removeClass("disabled");
                    }
                    else if (ret.error === "dir-not-found") {
                        alert("dir not found");
                        te.removeClass("disabled");
                    }
                    else {
                        var byteArr = base64ToArrayBuffer(ret.ret);
                        te.removeClass("disabled");
                        saveByteArray(fname, byteArr);

                    }
                    te.removeClass("disabled");
                    console.log("files result = ", ret);
                });
            }
            $(".repFiles").append(te);

        }



    }

    );
            //debugger;




    // aggiorno i combinabili
    
    if (gSuggested.length > 0) {

        $(".templateTabbleSidebar").each((iel, el0) => {
            let el = $(el0);

            let tabbleIdStr = el.attr("tabble");
            let tabbleId = JSON.parse(tabbleIdStr);

            if (gSuggested.some(su => su.pgName === tabbleId.pgName && su.pgIdOwner === tabbleId.pgIdOwner)) {

                el.find(".tabbleText").addClass("grassetto");
                el.find(".tabbleText").removeClass("semitrasp");
            }
            else {
                el.find(".tabbleText").removeClass("grassetto");
                el.find(".tabbleText").addClass("semitrasp");
            }


        });
    }
    else {
        $(".templateTabbleSidebar").each((iel, el0) => {
            let el = $(el0);

            let tabbleIdStr = el.attr("tabble");
            let tabbleId = JSON.parse(tabbleIdStr);

            
                el.find(".tabbleText").removeClass("grassetto");
                el.find(".tabbleText").removeClass("semitrasp");
            

        });
    }
}



async function initAsync() {

    uname = $("#hidUname").val();
    pwd = $("#hidPwd").val();



    $("#btnClearSearch").click(() => {
        $("#txtSearch").val("");
        rebuildAtb();


        rebuildFilePanel();

    });

    $(document).on('keypress', function (e) {
        if (e.which === 13) {

            if ($("#txtSearch").is(":focus")){
        //debugger;

                forseAggiungiIsTagged();

        rebuildAtb();


        rebuildFilePanel();

    }
        }
    });

    //$("#txtSearch").bind('keypress.enter', () => {

    //    //console.log("enter");
    //    debugger;
    //    rebuildAtb();


    //    rebuildFilePanel();


    //});




    //$(document).bind('keydown.ctrl_f', () => {

    //    $("#txtSearch").focus();
    //    //rebuildAtb();


    //    //rebuildFilePanel();


    //});



    console.log(`jquery ok. prefisso web api = ${prefissoWebApi}`);

    gTemplFile = $(".templateFile").first().css('display', 'flex').clone();
    $(".templateFile").remove();



    gTemplateAtbPlus = $(".templatePlusAtb").first().clone();
    $(".templatePlusAtb").remove();


    gTemplateAtbTabble = $(".templateAtbTabble").first().clone();
    $(".templateAtbTabble").remove();

    {
        let url = `${prefissoWebApi}/api/getFixedUserData?uname=${uname}&pwd=${pwd}`;
        let ret = await doGet(url);

        if (ret.error === "machine-name-to-impersonate-not-set-for-organization") {
            alert("You haven't set the machine name to impersonate for the organization");
            return;
        }
        else {
            //debugger;
            console.log(ret);
            gUserData = ret.ret.dataTables[0][0];

            // questo si legge da un'altra parte
            gUserData.machineName = ret.ret.machineName;

            gTabblesWebRows = ret.tabblesWebRows;

            gBrowser = ret.browser;

        }
        
    }

    //let udWorkspaceTabble = gUserData.udWorkspaceTabble;
    {
        let inp = {
            uname: uname,
            pwd: pwd,
            includeTagsOfSubordUsers: false,
            machineName : gUserData.machineName,
            workspaceTagName: gUserData.wsTagName,
            workspaceTagIdOwner: gUserData.uid /*gUserData.wsTagId sbagliato! qui devo passare l'id owner, non l'id tag*/
        };
        let url = `${prefissoWebApi}/api/getDataToComputeSuggested_client`;
        let ret = await doPost(url, inp);

        console.log(ret);
        //debugger;

        let templTabble = $(".templateTabbleSidebar").first().css('display', 'flex');
        $(".templateTabbleSidebar").remove();

        

        // disegnamo il side panel
        $.jstree.defaults.core.themes.variant = "large";


        let tabblesInWs = ret.ret.stcTabblesInWs;
        let childrenOf = ret.ret.stcChildrenOfAk2; 
        let initDataOfTab = ret.ret.stcInitDataOfTabble;


        //debugger;
        let fr = tabblesInWs.map(function (ta) {
            return {
                tabble: ta,
                elPadre: $(".ulOuterSidePanel")
            };
        });

        //debugger;
        for (; ;) {
            if (fr.length === 0) {
                break;
            }
            else {
                let curTabble0 = fr.pop();

                let curTabble = curTabble0.tabble;

                let li = null;
                if (curTabble.pgName !== undefined) { // se è un tag...

                    if (curTabble.pgName !== gUserData.compTagName && curTabble.pgName !== gUserData.favTagName) {
                        li = $("<li>")
                            //.addClass("fas fa-circle")
                            ;
                        let te = templTabble.clone();
                        let tabbleStringified = JSON.stringify(curTabble);
                        te.attr("tabble", tabbleStringified);
                        te.find(".tabbleText").text(curTabble.pgName);
                        te.find(".iconaPlusOuter").attr("tabble", tabbleStringified);
                        let initData = initDataOfTab[hashTabble(curTabble)];
                        te.find(".icona").css('color', `rgb(${initData.uiuTextColor.cR},${initData.uiuTextColor.cG},${initData.uiuTextColor.cB})`);
                        li.append(te);
                        li.appendTo(curTabble0.elPadre);



                        
                       
                       
                    }


                }

                if (li !== null) {

                    let ht = hashTabble(curTabble);
                    let figli = childrenOf[ht];
                    if (figli !== undefined) {
                        let ol = $("<ul>").appendTo(li);

                        figli.forEach(f => {
                            fr.push({ tabble: f, elPadre: ol });
                        });
                    }
                }
                
            }
        }

 

        $('.sidePanelDivJstree').jstree(
            {
                "core": {
                    "themes": {
                        "icons": false,
                        'name': 'proton',
                        'responsive': true,
                        dots: false
                    }
                }
            });


        $('.sidePanelDivJstree').on("select_node.jstree", async function (e, data) {
            //alert("node_id: " + data.node.id);
            //console.log(data.node.text);

            // estraggo il testo
            var el = $(data.node.text);
            var strTabble = el/*.find(".templateTabbleSidebar")*/.attr("tabble");
            var tab = JSON.parse(strTabble);
            console.log("tabble cliccato = ", tab);



            // estraggo il colore
            let colorStrRgb = el.find(".fa-circle.icona").css("color");
            //debugger;

            // ora devo chiamare web api: computeSuggestedTabblesAndCtsOfOpenTabbles


            
            if (gSuggested.some(su => su.pgName === tab.pgName && su.pgIdOwner === tab.pgIdOwner) // se è combinabile

            )
            {

                if ( // non  è già aperto
                    !gOpenTabbles.some(x => x.tagIdOwner === tab.pgIdOwner && x.tagName === tab.pgName)) {
                    //debugger;
                    gOpenTabbles.push({
                        tagIdOwner: tab.pgIdOwner,
                        tagName: tab.pgName,
                        colorStr : colorStrRgb
                    });
                }
            }
            else {
                gOpenTabbles = [{
                    tagIdOwner: tab.pgIdOwner,
                    tagName: tab.pgName,
                    colorStr: colorStrRgb
                }];
            }

            


            //if ($("#chkTestCombine").is(":checked")) {
                
            //}
            //else {
            //    ltags = [{
            //        tagIdOwner: tab.pgIdOwner,
            //        tagName: tab.pgName
            //    }];
            //    gOpenTabbles = ltags;
            //}
            


            rebuildAtb();


            rebuildFilePanel();

        });
    }

    

}

$(function () {



    initAsync();
});


//init();