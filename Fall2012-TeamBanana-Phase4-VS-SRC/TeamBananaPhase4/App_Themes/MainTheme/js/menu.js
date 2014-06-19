/* ==========================================================
* Menu JS written by Jared Short
* Write for IU Committee Project
* ========================================================== */

$(document).ready(function () {
    $('.sub-menu').hide();
    $('.dropper > a').toggle(
    function () {
        $(this).parent().find('.sub-menu').slideDown("fast");
    },
    function () {
        $(this).parent().find('.sub-menu').slideUp("fast");
        //  If something Slides Down, it should slide up as well,
        //  instead of plain hide(), your choice - :)
    }
);
});