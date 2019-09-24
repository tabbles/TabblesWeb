$(() => {

    $(".deleteDriveLetterInfo").click(function (e) {

        let confermato = confirm("Are you sure you want to delete the Drive Letter Info?");
        if (!confermato) {
            e.preventDefault();
        }
    });

});