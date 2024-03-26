// VALIDATION
(function () {

    'use strict';
    window.addEventListener('load', function () {
        // Fetch all the forms we want to apply custom Bootstrap validation styles to
        var forms = document.getElementsByClassName('needs-validation');
        // Loop over them and prevent submission
        var validation = Array.prototype.filter.call(forms, function (form) {
            form.addEventListener('submit', function (event) {
                if (form.checkValidity() === false) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    }, false);

})();

$('.radio__filter input ').click(function () {
    var demovalue = $(this).val();
    $(".radio__content").hide();
    $("#show" + demovalue).show();
});

$('.preloader').delay(1000).fadeOut("slow");

//Data picker

//var today = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
//$('#startDate').datepicker({
//    format: 'dd/mm/yyyy',
//    locale: 'es-es',
//    maxDate: function () {
//        return $('#endDate').val();
//    }
//});
//$('#endDate').datepicker({
//    format: 'dd/mm/yyyy',
//    locale: 'es-es',
//    minDate: function () {
//        return $('#startDate').val();
//    }
//});

$('#startDate').datepicker({ format: "dd/mm/yyyy", locale: 'es-es' });
$('#endDate').datepicker({ format: 'dd/mm/yyyy', locale: 'es-es' });


$("#InsHora").on("click", function () {    
    $(this).parent().find(".gj-icon").trigger('click');
});

$("#InsHora").timepicker({
    'timeFormat': 'H:i:s'
});

$("#UpdHora").on("click", function () {    
    $(this).parent().find(".gj-icon").trigger('click');
});

$("#UpdHora").timepicker({
    'timeFormat': 'H:i:s'
});

//INPUTS
(function (e, t, n) { var r = e.querySelectorAll("html")[0]; r.className = r.className.replace(/(^|\s)no-js(\s|$)/, "$1js$2") })(document, window, 0); 'use strict';

(function (document, window, index) {
    var inputs = document.querySelectorAll('.inputfile');
    Array.prototype.forEach.call(inputs, function (input) {
        var label = input.nextElementSibling,
            labelVal = label.innerHTML;

        input.addEventListener('change', function (e) {
            var fileName = '';
            if (this.files && this.files.length > 1)
                fileName = (this.getAttribute('data-multiple-caption') || '').replace('{count}', this.files.length);
            else
                fileName = e.target.value.split('\\').pop();

            if (fileName)
                label.querySelector('span').innerHTML = fileName;
            else
                label.innerHTML = labelVal;
        });

        // Firefox bug fix
        input.addEventListener('focus', function () { input.classList.add('has-focus'); });
        input.addEventListener('blur', function () { input.classList.remove('has-focus'); });
    });
}(document, window, 0));

$('body').on('change', '.inputfile', function (e) {
    var label = this.nextElementSibling,
        labelVal = label.innerHTML;
    var fileName = '';
    if (this.files && this.files.length > 1)
        fileName = (this.getAttribute('data-multiple-caption') || '').replace('{count}', this.files.length);
    else
        fileName = e.target.value.split('\\').pop();

    if (fileName)
        label.querySelector('span').innerHTML = fileName;
    else
        label.innerHTML = labelVal;

    this.addEventListener('focus', function () { this.classList.add('has-focus'); });
    this.addEventListener('blur', function () { this.classList.remove('has-focus'); });
});

//MENU LEFT
var open = true;
$('.menu__left').mouseenter(function () {
    if (!open) {
        showMenu();
    }
    else {
        hideMenu();
    }
});

$('.menu__left').mouseleave(function () {
    hideMenu();
});

function hideMenu() {
    $('.logo').removeClass('logo--bg');
    $('nav').removeClass('menu__left--open');
    $('.container__menu').removeClass('menu__push--right');
    open = false;
};

function showMenu() {
    $('.logo').addClass('logo--bg');
    $('nav').addClass('menu__left--open');
    $('.container__menu').addClass('menu__push--right');
    open = true;
};

hideMenu();

//timepicker
$('#HoraInicioOperacion').timepicker();
$('#HoraInicioOperacion').on("click", function () {
    $(this).parent().find(".gj-icon").trigger('click');
});

$('#HoraFinOperacion').timepicker();
$('#HoraFinOperacion').on("click", function () {
    $(this).parent().find(".gj-icon").trigger('click');
});

//$(".fchLlegada").datepicker();

$('.fchLlegada').each(function () {
 
    $(this).datepicker({ locale: 'es-es', maxDate: new Date });
    if ($(this).val() == "01/01/0001") {
        $(this).val("");
    }
});


$('.fchLlegadaT').each(function () {
    
    //$(this).datepicker({ locale: 'es-es', maxDate: new Date });

    $(this).datepicker({ format: 'dd/mm/yyyy' });

    if ($(this).val() == "01/01/0001") {
        $(this).val("");
    }
});

$(".fchLlegadaT").each(function () {
    $(this).datepicker();
});


$('.fchLlegadaT').on("change", function () {

    $('.fchLlegadaT').val($(this).val());

    $('fchLlegadaT tr').each(function () {
        var chkValAnt = $(this).find("td").eq(11).find(':checkbox');
        if (typeof chkValAnt.val() !== "undefined") {
            var arrCheck = chkValAnt.val().split(",");
            var valNew = arrCheck[0] + "," + $('.fchLlegadaT').val() + "," + arrCheck[2];            
            $("#chkAprobar-" + arrCheck[0]).val(valNew);
            $("#chkRechazar-" + arrCheck[0]).val(valNew);            
        }            
    });
});

$(".horLlegada").each(function () {
    $(this).timepicker();
});

$('.horLlegada').on("change", function () {

    $('.horLlegada').val($(this).val());

    $('table tr').each(function () {
        var chkValAnt = $(this).find("td").eq(11).find(':checkbox');
        if (typeof chkValAnt.val() !== "undefined") {
            var arrCheck = chkValAnt.val().split(",");
            var valNew = arrCheck[0] + "," + arrCheck[1] + "," + $('.horLlegada').val();            
            $("#chkAprobar-" + arrCheck[0]).val(valNew);
            $("#chkRechazar-" + arrCheck[0]).val(valNew);
        }
    });
});

//editor text
$("#editor").editor();

$.extend(true, $.fn.dataTable.defaults, {
    //"lengthChange": false,
    "language": {
        "sProcessing": "Procesando...",
        "sLengthMenu": "Mostrar _MENU_ registros",
        "sZeroRecords": "No se encontraron resultados",
        "sEmptyTable": "Ningún dato disponible en esta tabla",
        "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
        "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
        "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
        "sInfoPostFix": "",
        "sSearch": "Buscar:",
        "sUrl": "",
        "sInfoThousands": ",",
        "sLoadingRecords": "Cargando...",
        "oPaginate": {
            "sFirst": "Primero",
            "sLast": "Último",
            "sNext": "Siguiente",
            "sPrevious": "Anterior"
        },
        "oAria": {
            "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
            "sSortDescending": ": Activar para ordenar la columna de manera descendente"
        }
    }
});

var tblVuelosHistoricos = $("#tblVuelosHistoricos").DataTable({
    initComplete: function () {
        this.api().columns(2).every(function () {
            var column = this;
            var select = $('<select class="d-block" id="selectAerolineas" style="width:100%"><option value="">Filtrar por aerolínea</option></select>')
                .appendTo($(column.header()))
                .on('change', function () {
                    var val = $.fn.dataTable.util.escapeRegex(
                        $(this).val()
                    );

                    column
                        .search(val ? '^' + val + '$' : '', true, false)
                        .draw();
                });

            column.data().unique().sort().each(function (d, j) {
                select.append('<option value="' + d + '">' + d + '</option>')
            });
        });
    }
});

$("#tblVuelosHistoricos").css("width", "100%");

//$("#tblEntregados").DataTable({
//    select: {
//        style: 'single'
//    }
//} );
$("#tblEntregados").DataTable();

$("#tblRecibidos").DataTable();

$("#tblConsultas").DataTable();

$("#tbltckvuelos").DataTable();
$("#tbltckcobros").DataTable();
$("#tbltckanexos").DataTable();
$("#tbltckfactura").DataTable();
$("#tbltckpqr").DataTable();


$("#Usuarios").DataTable();
//var tableP = $('#NotificacionVuelosProcesados').DataTable();
var tableP = $('#NotificacionVuelosProcesados').DataTable({
    "dom": 'Bfrtip',
    "paging": true,
    "pageLength": 10,
    "ordering": true,
    "info": false,
    "bLengthChange": false,
    "order": [[1, "desc"]],
    "language": {
        "lengthMenu": "Mostrar _MENU_ contenidos por página",
        "searchPlaceholder": "Buscar en Aerolineas",
        "sSearch": "Filtro",
        "zeroRecords": "No se encontraron resultados",
        "info": "Página _PAGE_ de _PAGES_",
        "infoEmpty": "No hay información disponible",
        "infoFiltered": "(Se filtraron _MAX_ total de contenidos)",
        "paginate": {
            "first": "Primero",
            "last": "Último",
            "next": "Siguiente",
            "previous": "Anterior"
        },
    },
    columnDefs: [
        {
            targets: 0,
            checkboxes: {
                selectRow: true,
                selectAllPages: true
            },
        }
    ],
    //select: {
    //    style: 'os',
    //    //selector: 'td:first-child'
    //},
    buttons: [
        {
            // Excel
            extend: 'excelHtml5',
            //customize: function (xlsx) {
            //    var sheet = xlsx.xl.worksheets['sheet1.xml'];
            //    $('row:first c', sheet).attr('s', '#ffb400');
            //},
            excelStyles: {                      // Add an excelStyles definition
                cells: "5",                     // to row 2
                style: {                        // The style block
                    font: {                     // Style the font
                        name: "Arial",          // Font name
                        size: "14",             // Font size
                        color: "FFFFFF",        // Font Color
                        b: false,               // Remove bolding from header row
                    },
                    fill: {                     // Style the cell fill (background)
                        pattern: {              // Type of fill (pattern or gradient)
                            color: "#ffb400",    // Fill color
                        }
                    }
                }
            },

            title: 'Soporte de Notificacion',
            text: '<i class="fas fa-file-excel">Excel</i>',
            titleAttr: 'Excel',
            exportOptions: {
                columns: [2, 3, 4, 5, 6, 7, 8, 9, 11]
            },

        },
        {
            // PDF
            extend: 'pdfHtml5',
            orientation: 'landscape',
            pageSize: 'LEGAL',
            //alignment
            customize: function (doc) {
                doc.pageMargins = [150, 10, 5, 1];
                doc.styles.tableHeader.fillColor = "#ffb400";
                doc.styles.tableHeader.alignment = 'center';

                // doc.styles.tableHeader.athleteTable.alignment = 'center';
                doc.defaultStyle.noWrap = true;
                doc.content.splice(0, 0, {
                    margin: [5, 15, 15, 15],
                    alignment: 'center',
                    //alignment: 'left', // Izquierda
                    image: 'data:image/png;base64,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',
                    //image: 'data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAASIAAACgCAYAAACosWHRAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsIAAA7CARUoSoAAAC9eSURBVHhe7Z0JmBxVub+rqpdZkhBAgiAuLMpFgUAABQQuIBIQBUFBRFG8ymJCdpar4iVyRWUxhEzCvgZBdtkFAleMIoiACdsFFDAswv8iEAJkpnumu+r/e6urQ89MdU9XT68z532e7+nu6r27zu985zvf+Y7teZ5lMBgMjcQJLg0Gg6FhGCEyGAwNxwiRwWBoOEaIDAZDwzFCZDAYGo4RIoPB0HCMEBkMhoZjhMhgMDScNQmNtm37l4Ya43m2fuyiWaQzZ878SiwWm5vNZlP6c8bZjvPJ/H+UTCatdG/v0vb29i+ffvrpq/yDA5gxY8b32tvaLu7r67Ncz3tB/+qbenZW/+9zsnf1uv/b1dW1KHi4wdBQdG77J7cRohqx++67x7f6zGfWT/T1babf+MOu664nIZmYyWRW6LLrzDPPfDd4aD9mzZp1rIRkUTqd9m/n/x8oU4hObEsmT0eICv9TrvP8VE/P8rO7uiYFhweh52+lx+6sqy/pvV+TrVi4cOE7uXsNhuqi88s/wc3QrMqoIW8/a8aMc7fbbrub4729N+l3vjHmOL8e09nZJYE5UtLwjZ6envWChw8i43mvp1KpDMIR/EeR0PPWPInnF5q8Icuz7X8Ed4fiOM7XOzs6LtTr3ODY9m90+2Z9n2vkqZ2u7/YNXf5b8FCDoWoYIaoyasAfd2KxKR3t7V9MJhKficdiExABiQveCIKwrjyTtYOHD8J23ZSGVH3BzcgUCtFA5JVx8X/+jRDmzp3r6PNt3tvba0k8x8bj8c3kXe3Z3tFxqL7LiXHHuVgPO1aPMe6zoaoYISqT4447bn15BLvOmTXrm1OmTFknODwICcGTWdd9sU+NmeGR74VIiEACg8exvo4XFaJYLPaWWnmKYVQikfCHU21tbWtMjJWoFRUCic2Y/GN5PiZB4X2DRxQXolNOOQXX6WNc53NrGGkhSogot2PxeIe+y8pSYnf88cdvpuHlpsFNg6EsjBCVQMOQjWWHzJ4582fZTOY6DVV+n0gmr5Q47BQ8ZBDjx49/VhevqbHmDgiu52+3t7dLE5yP+DdCkEi9o0f+pbev73FdfzTd23tXOp2+MZVO3ySP6hYJwZ1rr712b/DwfgSeyvMa+t2kx9+u13hEtkyv8zcJySrZO/ocobElmDNnzgd0/8cKP3sexBSPSp/9T8GhQRx99NEJidd5eszts2fPXqTf7qjp06cXjUcZDHlMsHoANGY1osN19XO6MUm/zpYaZsXxCDCQx/PzBQsWnOTfCEEN8Kr2trZv8Pj878slv7Eu03r+dD3/Iv+OAZxwwgnjVmcyH8T3UcPPSFRWWttv39325JPOO++8Y8vTyV544YXFhm62Gn7y77qyZTIZ18V6+gwxqVZHwvM+oNuI4Cvz589/jgcPZNq0abtoSHanhHYcohN8Xv8+rmey2f+Tx7bjvHnzXvQPDkCe0Hae697R0dGxAbf75FG5mcwLGhA+pec/qte6T9/nwRKf39Bg3AcO7rDWSm9kZdx2y+2LWR3vveB88oHQiZVqoHPCP8GMEA1g7ty58bfffntJZ0fHnvKCaHz52IqPGiLH/iKvZBcNZTLB4X5IDKaoQf+3rnZrOLZCbudyScATuv2CPIZX9Vu/2owzUfKIEK69dHVLnReb6nNupbNiA13v1BBvnITldxKZg0877bSVuWf0R0PXafIauyR2dl7I+L0wRFme2cu243zl7LPPfiR4iqHJcJfvvb7lORdbY2Lb66Ztre5bbG97z4/ok3KPqC5GiEogj+aYRDx+fqFHk8dvVJnM25Km3bu6uh4PDvdDQjQhHo9vINF5cd111+0uJlhNjq2hVlwe2Af1nbfQbab0X9DQ89qw70Oge9XKlZe1d3R8m5jSQIhZabi4TK/15bPOOuvl4LChydDpbnuP7dNlr5WYxg2vO/ue1ecd7mx39y3BQ6rKqBWiKVOmbKqhx0dfffXVP15//fW5sdYAZk+d+nEvmXxAvfuEfM/Or6Pe3A/8IlASo+/PX7DggtwzDBLfzfR73d7Z2bkFOVCcV4UijhCle3t/IW/oR8GhQcij+oSe8wF5i38ODhkagLyiHeUV/dZuc9a1Yo7EqO8JK2590dl6SdU7EP3f/kkyaoLVaiifkqfzCw25btbNSzbaaKOiQdRMLPayhlRLmLHKzzhlPS+bdd2n1ZjO1RDla042uyR4uEFo2PWuFPuqVE/PnyTefflhGb+fvEtL4kSc4cHcowdz8OzZHXrOf0qwrtf/dLnswOOOO25McLehjjjb3vOQet7H9H9aVipj2WPiW1tZ53Tvvt2JO9aEEe0RaWihDrpzkr7j9yW9e6lRbEjD4Bv39vb+TL3zj3OPHIyE61sSrbM1nHhFar1E4nRT1nFe+NeLL/6rmCdlsAj0ryMh+qBOrMmebe+t332iBP1DvX19D+n4F4rFxuQNbSXB/31bMvkBvFANa1d7rvt39QJXyAO9VUL2kgly1w932eT/sGP2pb4YgRqN1+dOd7ZfUtXlQXmPaEQKEQHnlStXTpboHOFms1/WZRvfkxOcS3JrJERPyev5shrG88HT+jF16tQN2mOxTb14fNn8+fN7gsOGiEiYNtJvfqDs9QULFlwfHB6ILQ9oejKRWCAB8v8jvCnOSf4z/U9v6tjV8pZOP+OMM14JnmOoIe6jn/+QOoFH7KSzodUnjWiLIUSvW9nsV5xJ9xRN4YiK/teRK0Tks3S2ty8cM3bsMT09Pf1mvYDvyomeyWaPkldEtrChgUj0x2r4tlQdxnbE3wrhv2KInEqlXrcdZ2f9Xy8EdxlqiOfNdbzHHuyyO+LHWj3BfzJGo4n3+v5oxdoOcSbeVjQxNgp5IRqRMSJceDeRuKi7u/sl4juFcDsfu5BCfVE9cdEsZ0N9kKezqwRnEv8LNhC8JCnSFUaE6odtn+Lajn2z1+tmLCdwUiRIdmd8NyubmpU7UD1GbLC6a968RyW1d9CjYghQm3pWFLivr++5nlRqQV82+9O11177veAphgahYfLjcs1PSPf2Pizv9V0N0fzhM/DfSYje1d92oX+gCMceeywJm4Yh8J7cfay7/Aube//40hbest1LdsJeu/2YlXH/bLUHnYOrFpWROc5x3rJ9D8kdrA4tOzSbMWPGQTpVX5rX1fVocGgQs2bN2sVz3d+O6excq7unx5Nr/4Bc/xslSteod30teJihSZg7d27yrbfe2kde0YHqMXbVkGxzPNfu1asvWvbYY1OXLl0amo/1gx/8YB15v9foeU/2et7V55iEyaIgQpaVvdsen9zYe7vvLjub+Ja9w+1vBHcPwl2+zwx7bHyBtVo/fU4qcvGidOZ5y4rt52x759+Co/3wnjwk6SXftZ3N78rVsykCjgGXLSdEGkqRWDfVse2D1Xs+3d7RsWex2jygx1+g3nXDTDp9fTqTuf28884LzQo2NBdTZ83aMuF5ByTi8b2yrvvT+fPnLw3uGoQ6nNkxxzkLLyqVSr2qM/panc+/VmdjBCkE97F9bpC4fNXKSgi6+6602tqnOp+8NXQZh7t83/3lCV1stzvrW71BrBWpSGgwlXaXepZ3lVTEtW3nI17M7rToKjx3nB40wYpJVLLuSjvhLbK3ume5/9wBtJwQTZ069WM60abpux2WSCY38pMKmVFx3aOLrduC2bNnr7t69WrWZxUVK0PzQpb6uuuuu+qUU04JXejLrJybzd7a1ta2HZUC4sSZcnlL5IJdpnPm0pdeeukVk3LxPu6jkw+1HOsKuz2e1NBLuuHdqIa0QirQIfuIFbM6rayvCyx5XFfHPmrH7LH+0CxPIEZeWgelIvZY3UhKnHgImsLz0ZQ2Peb19J2W3fkdZ9ubXvefW0DLCBEnosRnP8+2f6TeUW5lELwUuO29fX1P6aTb54ILLvinf9AwmrA1RJ+VTCTOomMqPJcJejNbqmOvSpCOV2d1tX/nCOaQQw6Jbbzxxp3yCtvV+aYuvfTSUC/He3zXdTx3DFPzm/qCQTCa345LPB0/OK3b/JzEhPCECkUoDw/LB7K5P+QhxJe8nuzrdszbz564ZFAYJS9ETR2sZv2SOCmeSFyunm5zTra8CAG3dRJuqd7wP8kdCg4bRgkadn9MQ/SjGZLlRQi4znmCINmOQyWA/vkbIwz9DttKkGdutNFGC/v6+q5Um7lxrbXWOiC4exD2xPtX6kda4v8qfgAaodF1RKdb7evdPst6T5fEhdJyJMNECPKHEaO4pASPqCP2vo1VkxxHs/T+LIUrOePZ1EIU8Kx6tHzvFhx6v9fTicjN8StWrDBCNMqQR9ytM+LKVDr9EmLE+VDo2XO7L5u9LplM/jY4NFLZsaOj4+y2ZHKK7AB12ruprZSeRbT9CFHuui8kgeERIShc5i3v9RTCIT3ey7rdXia7wuvL/tPrzT7vdWfu9a0nc6/Vnb3e+1fqDI27TrIn3lEyNtsKQ7M2Dc0ua2tvP4zFlMCQDGGSQC2TV3SueoFbzz///H/prveVyjBqmDNnzuY6Hw7VuXyUxOcjdE6cz/KKXtcJQdmRqmUCNxJGCKeccsog727atGmbxBznKX33Dm6rTbjyBGfoe5/jP2AA3n1W3Ft78v12W2xH9e6W1+vJ9fGe1o+2UgKjsZT3tlzJl9WaNN511ejs/ex2ZwsrXfDWDLlS2actN3OE3JlVdibmePrJrUw2JzjjZC87KWefe1b7t4ug/8xvsy0RrJ41a9YOuripPZn8MN5Rb2/vi/rUl+uzn9fV1VWVDE9D6+MHrl33GJ3Lh2u4vkkqlTpnwYIF04K7B0Enp4t9E6779FnnnBM6Dd0MUCdKF9+VsG743nvvnTww9kNmury+JRKjnWnPQwmR+/C+m0mKHrA7Y+t7rsSjx51p9VpXWu25tHZ7fG/Wenj9PuuQT3nWE4+M9zJ9v7HXSuzhD9cADwkBy1pHOdvdPayVCXkhkt/VWAiwHTNjxie4DA4NgmlYT56PROjNdG/vr/Qj768T7CdGhAyFzJ8//586L07Wuf0FidBcec7zgrtCcRxnD3kRN3uJxG3q7I6UkPkeRTMhsdxf3+daDbdOl9BMGzdu3K7BXWuYMGECBfj+pO+TXznAJghF25MVd3eS37G+1Rm3rHTmGutf2cXOTne942x7z2rM3mRpyv7a9VmyqzXs+riesbnVV+ANaeimIdl99qQ3FgdHhk1DPaKZM2durIuT9CPvmHXdKaVcaHoFDcG2XLhw4R90M/ehDYYKUQNfS0O43yYSiV24jactT+I2Xf2lxIxzrKGQKR53nBNjicRUtY+xOvfzs8RLdPmVefPm9RvyzJ4x46D2zs7fSIDf1nd5Xe35VLWnXwV398Ndtu9stfzjNQx71ra9qfbEJc8Edw2CVfjWmPgldk82V6Mx5pcPTusHO9DZdslduUdVjl6rsR6RROgAfYobk4nEkbKt9Xl+UipN/6yzznpDIkRSmxEhw7CR50A5312YefVnYtUe2tva9pc43SDvaC7nos7JhsQr6HQlNpdpeHmiPs9Y8qNor3xO3Z7cm8kcGjx0DW2dnb+XAO0l+6xu7qPvVbyiop29ynKze9h9qa+VEiE1NL7/NnZSY7F8q+uUo5W1rrW32bmq9bjq7hHRE9mx2HHyHeeoNxrrnwSCGQ6p/Zkabp3oHzAYagRLf3S236DGvgGeRiF4HX6jz2afUJs4TF7FU8FddYPYlYTyl23J5DQ+X76NgsSJkrvPJ5PJScV2C64W3kP7beAlMtfbnfFdrVTWstpJYMy+IRnf39nmrqpU0dR3879cVT0i7/G9NvUe3muP4OYg9AOzwnpxMhY7WT+0727yOfImtZ+qcfpBwcMNhpqgc22C7TgZmR9TKYSOkU5RPfQ4XW9INrY8/7Q+46kSnDtJS8gj8aGzfltXb3Vdt+bxLM/JfshK2tv48SF+JpwV17miWiJUSNWEyP3z3pt4bvxX3tjk9d6yyQcGh9fgj8kd54LOjo4DcYeZfh+IfvQxOr5ncNNgqAkLFiy4Wefg/uoIL+Y8xAvKjwgQJgkQE9U/XLRoUdFhS61hIkZiNF3DsmfwgviM+lxLdOyr+vxz5s2bN2i5RNVJWFvabfFxfkJjMkYh/efUcM8L7q0qVREid/lum9jt9mX2mNhn7ZiznufYl7kP731wcLdPOp3uUS9zdSqVyhYOA7mO6uvEeLOvt3eOfvBTg7sMhpqhxrz8lVdeOVZN7CA18L/Q0KmtHZc3pGHZpa+99lqxapJVhdnio446asPgZj+oHqrGMlNe0Ap9xnk9qdTXNVT8XXB3TfEe2T4ht/CzftY10/V4RZ53vrP9ktA98YbLsGNE7vK9N5Evu9huj+1m9WR0QAdJdup139KHn6oPfm3ukZZ13XXXxR544IEz9GfPwSui98HSvb1/0Oc4OQhGGwx1he3EdT7OkRD9h7ykN7KuewAiENxdMxChDTfc8GS97wGZVOr7C84996Hgrn4wuyzhXBHcrAvu0weMs9KpB+1kbEuGZF5P9kkr6e3ubLXkreAhVUHt3hegYQmR+8S+m1lZ70p7bHynfvVKADHKuiut7syxzqfvXbPgcPoPfzjB6e7+9ZjOzs/39PS4ructlLf08/PPP7/2rqbBUILp06fvqXYQ17DonuDQIMhuXrly5VT1oOusWrnyzMWLFw/exK0MyFmS+B2vTvlkDb3iven0M25f3xHzFy36S/CQhuI+vu9E27Me82fJ5A15vd5hzrZ3XRPcXTWGLUTe45O38FzrCntM4tO+CIXBwreMt8rrzR7rTFpyVXCUombs7HBqbyp1S9eiRZcEhw2Gpkfn7t4axt0qT75d3tN1spMmTJjwQtjSi1LIy/muPCH/3NewK1+Xe4Ua4rckhOTTFXbrdccvFRK3f2zFvA4rY//OXpmaYe+5tCLRLUVeiCqKEbkP7bml59k5EWK1bjEouh2zx+tdLvQe2ec7wVECcY+/9dZb3zQiZGglZs2axTbc82ISIVfi0dHe/rVkInHjqlWrSMyNhOu6r8oj8muq4wSQK9Te3r6xY9tX6X0+FTysYdiee5eVsCZbfZnd7fG1EaFCIguR+/BeW9udbYvtzvin/ZjQULqdyloaZ3Z6Sesc9697fy84ahWrlWIwNCMnnHDCOHXeZ8iL2RoPhkxsvzP3vOfGjx8feYujhQsX3pXJZg9Np9OPUEsdMcL0mo9JoBoeprB3uHeV86m7X3Mm/e6fLPkIDteMSELkLvviRlYydok1Nr69Pxwr1xlNu5adcDqttvg5fsq4wdBiyGM5SJ7QV7mOADHLpqHUMjsWm1GseuRQSIz+7MRih3WnUjfLG7LSPT0P9WUyR+s4lSRGFdE8okRfjzygh/0hF3VKosAUoOu9aXmuWahqaDkkPg9mXfcSZnvJ65FX9IZcmFkstA0eEoo63s+4j+17rfvXySy9GISe/5xE7ahUOj07ZlnfPffcc/9fcNeoInKw2q/On1l1mh2PzfZXohSuyi1Gp58M9ZL+ze84k5bcFxw1GFoKNu4c095+lOU4J0qUTuvq6jo/uCsU9+F9trParcvt8cmtvbd7/1eHjnQm3v1g7l4DSH98Aapo1sy7zop5m+/7CztuneAfoMRkMdgdcnXfS1bW+5az/T0NX9VsMAyXGTNmfHCdddZ5V0Oy7uDQIHwR6rQX28nYVn4s1d+Cx33ayrhHOtsteSB42KhnWEIEnrd73FqWPNVKxv7TPzBQjHg5PKG0+3crm/m2s829VV+fYjA0I+6jn9/BSsZ/5Vc1XB0sV6M9kFvXnXneintHOFtXb//4ViYvRBVN34NtL81Y7lv/5WXcn/szZwNjRh3yhFLus9bqviOMCBlGC34sKOZcY7fH3hchoI0wgzwmsZmVda5wl3/hc7k7DDCszGrQ023v8X1+Yjn2j+2Y7fhbjzAc6848Y3W733F2vic0bd1gGGl4j3x+Vy8Ru9zuiG9WNL+OZtYhzyiV/YdGEcdomFY0i3s0kPeIhi1Eedzlk0+24s7J9th4zFvZ+6zluoc7299rdto0jAq8R/bd1Wv3LreTMYlQGdVDEKO0u8K2vKPsiXffGxwddVRdiPQqtocYtcf2sFK9c5xt/2dZcJfBMKJxl+092YrHLrLbnI+WJUJ5iBmlM/+0MiwOv/fW4OioIi9EFceIBiIZ8+xtxp9qvZk51IiQYbTgl8uw7IPstZMftVKu3yOXTZ9r2WslN7JizqgvBlg1j8hgGI2o+djeX/f8qBVr/6U91jnYT/bN7RtfGlwApvRTmTvsmDvV3urel3J3jC7yHpERIoOhCrjL917f9mKLrLGxQ3wxKrZNM7DKoM1BhG6zY+mj7a2WjspsasgLUdWGZgbDaMbZ9p7XrbfjU713e69FZNh2JxSOI0LdmVvVCo8czSJUiPGIDIYq4j2+6zqeO6bL7owd7m/RTKJvvmkhQgl/J4wbbbf9SHvSLRTCH9UYj8hgqAH2xPtX2l5qutWdXUwMyEpIfGhqDMfYITWdvdZ2U0aEBmCEyGCoMvakpW9b766ereHXZf6KA4ZqSZvh2NW2HZ/i32/ohxEig6EG2Lvdv9JK2bO81dnLrLhffP5q20pPtSfesTJ4iKEAEyMyGGqI98yX1vN6eveRCN1hPKHB5GNERogMBkPDyAuRGZoZDIaGY4TIYDA0nEqGZknZBFnMv1UerARkZ4I+/1bt4bOtI9tAxtYsW8s2lcVln5B1yAZCzduXZYzj35H9Q0Y94idkrwXHilbkqwMflLXlrpbNm7LVuatFGSNbT7aNbEvZJ2W810dleSjmTuIdvw8bAC4Pbq+SAR0a2yZHOSeA3/ON3NW6Mk7G+REFviO/Q7Hfk+++vizh3yoPaoXQLorUDKkJnPtry2gHm8loG/x3HN9cNlAIOO9ZfkIbzp8D2GMydn1lN56Kd/nID8244lsEPiNj/+t0BGP73nrs1fQx2ddki2RPydhdAYHhC1ZqvAY//G9klMbdS0ZDrTfseR7225ayr8uKwUnIjiq3yDihONHCvv9A4/fskVF7fLaM/5WTmNcJ+wyljMXRfI56gqDMl4V9nlLGd/53WTE+JPurLOy5xexpWeQ90SqADmxH2TQZe/ojKIhf2P9bjnEO8PmflbGL89EyfpsPyCKBEEElQrST7FXZwA9XyvAotpLVii1kp8iofxT2/tU0/oTfy34q48+tF9Q5Dvs8pexw2UA+IqO8Lz1a2HOiGp3ST2SXFRyLYqXEshbQWeHRhX2WUkaVUUYCxdhIhrCEPbeYsZ89nnot+YoMsSBtIOwzVMsY7dwtO1FGeywLX4VEJUJE48NVG/hBShkeBW5/tWFI8TMZyhz2vrU2vhcNcGdZrfmjLOwzlLJvyAo5VFYtARpobBNFLxl2Xyk7T8Zwv158WYaXG/ZZStlxslIgRAzjw55bzBDxTWS14Iuy22UMf8Peu5b2jOxc2faykvgqJFpZiPaV4dqHvV+9jdjBf8mixh2iMBwhYuj0c1kjTsqhjHOjVo1xIMQIF8jCPkcpYyjDSKAUzSJEhA34jsR2wt6znkbnxEilMN7YD1+FRCsKEcE0hhYESsPeq5G2RFYrV7sSIaLg1njZNQXHmtH8HVTrAGLxN1nYZyhl18rCJjgKaQYhwjNnMiHsvRpp7On2bdmgyRZfhUSrTd/jcaD2p8nW4kCTsbeM8TizD42GWBY9EUMfhmT1gBkwgrpRQYjqkVG7nYzZoigQxF8qq+R71ZP8ufdp/1ZzwUzsYhlhjH/jwCAkRq3iEXGiniULe/1mM3YuKeqOVkhUjwghYpIg7L5aGNPaBMeJDYTdX8oY+jDrVEs4fy6Rhb1/KSP+WM5/2UiPiBkrUk3C3qPZjNlsPHU/1cN3h0SrCBG5LmfKaFxhr9+MdoUsSk7JUFQyNKun/VgGe8qixqKYcfmOrJYwtfyiLOz9Sxm9eDk0SoiYBa1kFrCRRt7RwbI1QtQqQzOGZOQv1cN9rxaHyAbOWo1ULpAxXIb7ZcyaRIEgcq0LyE+WRfW6aDBX5a42Jcw2MklCMmorQdIwXvAaWkWIXpFNlV3n32oN2mXHyBqR/FhP7pAdL8tnB+PdXJm7GgkyfGuZ3PglGYIXBYZlD+auNiV4n/XOwxouCBCJtHRYa2ilYDVjy+/KSJhiarJScA1pLOSSDLRqp9oTOPx87uqIhATSmbL3/Fvvc6ds4LGhYIjxhdzVqkMSIx51VMhCHmqJTKPAG2ISguUq1YB2EdYmCNZXCzxMPDjOj35UstaMGBF/ECdOueDRkPeDmFQDerfTZeUsGyGjlJgWvduTMgK43CbeVPiluT1WxklLlJ/X5jo2HHDtj5INd9aFGNGuuatVhYaGq8wlsR1+A7w51k2VSjQkvnGYLGw3304ZsRWW20SBafJvyqp58gMdGEH0KGv1SA8hCPy4f2toiBHdJYuygoClT8x2sa4xKgzH8EZ530ohZka7eFTGZ8nHfvPtgo4Zj562jrfKjCOBe4a4/MdR4HV/IDvDvxVAfCh/pRWC1WHwo3Dihr0fhvr+WoYA0rAqganGH8qGs3TkBVk1pvOrHawmsHq2DLHYVpYfQnKSTZThPp8vI94z8Lk00v1kpThCNvB5QxlLHnjvasJwjImDsPcrZTfJhsodKqTeweojZWGvWY4hQCS4lr0UowC+J57YRTLyg8JeP8xY/1k0j6iVhQgQGBaismiz8P3wwOgFqzX0pBdgoWSls3b0esOlWkLEf8Hwttx8GjzDubJ8KgCeE41gKHj9qGuvMBZQVhMaWyVJjFFn8eopRHiql8rCXnMoY1RQLc+a33aWbKjvTWyXxNpB+CokWl2I8uD18APzXgzFahFrwF1lOFiJGE2XDZdqCNH/yCpNeJskY3/2X/i3hobfi14w7HOUMrzYqG5/KZjkKLeyQN7+Loua+FhPIWKNJcOpsNcsZbTDz8mqDeELJix4/YHvSTJo0cXCvgqJkSJEwInDUgZKU9QKsrn/IAv7jqUMN3a4OUXDFSKGGsNNGsS1jlJziIWXxJ/CPk8xIzu7WucK8a4bZWHvU8rIRi87aBpQTyGi4UddS4YYMxyrJR+Xca5To4j3ZC0ox4riq5AYSUIE1UwgLAZDQWbdwr5nMSOISaMYDsMRontlFL+qNxTgIss87DOVMobV1YB4E4XHwt6jmDH0pHRGVOopRIQKotYTIvOaFIl6QAyJjm9I78tXIdFqa82GAoGoNbiaxKSigGtaz1IXhdAQfyQjxlNv8IZYCExDiAJZt9XoVHaTlaohFAazZAh3M4N4RfXYqNZZrVnroWASif+QYn5lMdKEqB4wY0R+RRTyMYpGcLOMFdmNgpMSQYrCLrLhVi5kGLl/7mokiKMNJ0+tHhAjiipEeFDEN+tFpBQMI0TRIUYS9SRoFDSoi3NXGwa9cFiuUSkYxhJfGg7kvZQq7RoGni7C3eyQ5hC1YyONoWnbuxGikQ1JatRRbiQ0GPJ4osB5SSXF4ZyfCFmUPCBgJiqqaDYChllRO8NaZq4PGyNEIxsCxfV0x4tBXC1q9jA5KpUGVzmvD8hdjQQrBloBYqFUP4wCEwfUWa8kibHmGCEa2eTLQzQaNlugfnIUWGJSSYwHqJXMMp0oEMxndrMVYHq8ksAzuWDE7Fj/2KjJk1CMEI1smiXoSuCSHR6iBPk5Nyl9WkmDYQgSdWsbppsbMbNYCaQYkKNTCaQ0sPUTpVsY/uJ1srynHqkvRTFCFB1OgmbwMsqhmYLqzNyxJU8UyAKPmglOoHv33NWy4f/8raza1RdqBcLOcLfSYTeZ6yxhITDPSni8JBYpkwl/UmAs42FRMzlVzGLyPzCsozZY5P3LhqJVV98PBT80sybkWzA2/nBwbLgCwvPJS2GFeJTyCw/L9pGx/KRSSGiMukaIz8mSiWaBKo7EKaJAAukvc1fLgtyhG2QM7coFgaTBDccjIqGxnqvvaX8ICXW4awXCTDyK85brVGfgOg4MO9dwm8/OLB6Bfry0SGJOMmP+ykjIrGZ2BJfz+zL+HFa8U4Apvx1u2Geqp+ENDHeroUoyq5utQuRnZfwvYZ+1mFHqInTBZBHozcNep5SxqHe41DOzOg+VIcJet96GWCFMLHI+R0anW1YiKUIErS5ENG5KsrJ7QX59SzOaEaL3IWgd9lmLGQXWyh2erSvLZ3KXawTSq7FBZiOEiK2rmmVvv0JjyMjnokJpSc/UVyHRyjEiVJdFrpQYoFzmcNdyGeoDQhFlKQ4bJxB8LudcZaZsqI0QB8LOt8wutiJ4/mz33WyZ4MR5qHFFPSva516ykrSiEFGDiLVTxD4oiG5oLTgx+xVOL4NyS6ISpI4Su0MQCdY2+55lpWAGjB1umhX+ExwG6hYVDUS3mhAxvGHJAvvd44YbWg9E6J7c1bKhxAs9bCkQIEoIR4HPwrR9q8MOKv8ti7S+q46wNm6ejFKxobSSEOGid8mYCTK0NtTxjjL1TI7LUPWviUESv4wCcTfil60OM1XMRlKLi+UfzQhawzCSEsSDaCUhYr/70bJP2EiHqV7qHUeBBayl4oB4Q1HP56hr4JoZxGihjDYSNV+rXpCcimAOmrhqFSGitgn5JK0knIbiMMMZde8zZpeKbc1E3DDqchAWAzeyPEqtYDKA9nKyjPyeZoPZxTmyfvGiVmjYVKMjz6PSnTgMzQfDMopmkaVeLgzNi01OkJ/EVHYUqL89nATTZoZqjHgeBIoZDlHPPWpNqFpCZYR+w+hmFyJU86uyKNmqpSATlLwRiqNXYuwGwVYskbJHDaGwTVHUoDWbJG6Qu9oPhmVR0jfwyPAcRjrk+50iI0+KFBc2fyDhlxUOtIO0rBGwtg2vbY3+NPsSD9a04D5H7e3yUCb1PhnlP+klKMxO8auou5Dm4ceiGDgLBqMsITBLPMIh4S1KoXqy5NkvjRSAPMyesrQiypo01pWxjqqa+Tf1XuIxHNhnj7ZF9jMzWnibCDl1zVkKxSUjEM5xNozgvmrPUpMDtZP0hzbqZzY2c2Y1DSns9YYyarWwtS3vWc2taYBYBd8n7H2LmcmsDocGEXXPMfaXK6SSnULYYqjaNCKzuhZQyZH1mQgUu4XwH7FCH6+KdXx4VmwdRAdH5x72vcq1f5f2+DS7ENHzhb1eKcP7IGZQK9hwEA8v7L2LmRGi4vxKFvbZixnxjnw9azwpkvnCHlfMaPzlbFUelZEiROVCSgUeEwuZGWmEfb+h7Me+ColmjhHhErK/dxSI31C+4AH/lqEVIKGQ2F250KHlOzXiRVGHq/fLoqYOGAZDVjrDqlNlZL5HzZYHYn4+zSxE7DuPGEXhLBlrhwytA7tmELiOQr72MoI0VMZ1IQgeBdoM1YX/kHpGUesjrRklNbMQEVmPUvycMgT0drUGl9JQPdieKWqJViY+CLQSs4iS1kGA9LbcVUOVwbONGvwnBuXT7EIU5SQj6Bm1oHglMDYue4rRUBZsCx1leEahOwqZRV1bRm2jSmdMDaVhJBJViNZsX97MQsQUIpvklQu5PZWWzoxCforTUD1I66D0abkws3OiLEp1QoSOulUjEabiSVzcwb/VGGh/FY8WmlmIon4pXHXyIWoNeUSm9lF1IbEOr6hc6Emj5paxX9lIDFJTvZKUBlYfMANJfelGwAim4p1BmlmImBKMUkCLfIc1Y84awpYs9MiG6vIHWS0T+xbLopxPrQClTyh4/y3/Vq64PYJO1nK9IWUmSknffjSzEBHviVKwCnH4rqyW8Rtc4KgpBYbyIPWiVjNaTDP/KXd1xEDe0oWyw/1b74NnQpmVX8jClsPUAjwhdgWJGrLA2fBpdiEipT8KpO0fm7taE6iPXenuo4bSsOcZQhT1Py8HFrg240r0SsHzYZkVWc5hIAwUISMh+CAO1BhW0xerjFCKNduhN7MQEYVnXVgU8IZY2FeyLGWFEBhl14QoAXRDNKijw7rAaoLAkR7QqAWe1YaOkFhQOQX/SW9gKQYLXdlMcc0sVZVgVpulVGRXV1IdY80ERTMLEbMclQQXcQ9J+yfBioSp4QgHfxzu7RQZfyYlSQy1g+xcYkXVBGEbKZn2JG9yXkeZHUMgECHEi2VCeC8scRnOkiMWwB4oo3oCq/srmSRiMopEyBxec681Y6gV9nrlGosh+eN4HZYCMOPF9DtBPkQYoeGSWTByUz4ko8ehGiClSQkEUv4j7LWjmFlrVj7U0Kl07dJAI52DWsn1oNZrzRAhZv7CXieqUQeK84na7/m2QfvMtwESiRlR0InTETMJxIiABcaMCvBcw143ihGzG+cvNBPNXgaERXX0kCz3GC4Evim7gDgx5OOSL80PQLSfIB8nLn8GP37FU5EhmDIg5cN/QtG0PfxbwwNBYwYpSo5SpdSyDAgzwuwHxyYCtYC2wS63rE4gH4jrJH7mS4JwyferZvB7umwRIuTf4jJ/vUzq6REBw6Kw12wlMx5RNCjTwY4UYd8pitGJVRK7qIRaekS8NsOgsNdoRaNj9kUNIYJmjhHlYd8mXFLD6IH/vBplXHmdWszC1Rvq/tCpsJVWs24ZVC54XyxO77davxWEiJKWpK+bNUKjB05SqigOB3KHomRrNzsMm0hNYTebZqo/HRUmkgYttWkFIQIWKzJNOBJ6N8PQ0OsPN7ZFTeqRlDsEpCIQfCf2x5ZMrcY5sjNyV/vTKkIE7NnEDJhhdEAe2dO5qxUxkryhgeAtUn2ANWat0jkzA02J2dCdW1pJiOglyYFgypGeoVHQ09aj3Mhoh+FZYZH8KCyXNesmg9WCCSMaNtURqcPVrLEjQitMPrALbVHRbCUhAr4IWZxkTjdiTyp2BMG1ZDsaQ21hduX3MgqnRYVp9NzuECMb0k1YvsJ+bzNlD8maiRtkB8jYqaXkFlytJkR5+GKs+3o/M7P2PCj7vowp1yiVIw2VQ8OKWnWT4lxMddejNlWzwEwU8RcyqKfJGp1JTrtk2ycW5JYVy2pVIQK+LFmhx8mIJdTKNWWGgulGEuOoAkkWdtQ1O9VY41PJf1Xt9Xb1hgaGVxQFOgzyVBpBI86LQggZIEicqyzBuEbGbFs9RJnYD8ugaJPYFbLy1/d5zZ/QWA7swUQZgjtlzJTwA4R9jnKNISCic5GMsh+FDZp1Oox7w55XzMiDGm5CIynxYa9dyphdaXVYaU6t6bDvN9BocHRMjYCkQ1YOhH2uYsb3quV2QqwOYKXA92TUlKbDJqQR9lmiGmtBaWtkwRO7rWhdp5/NKJp9iUclcOLuJJsoY/8rKjciAhiL9Qp7B740yz1w5zEEFiPYyQ+M4AyE12BYyLKQcrwwej0S0hgvD2cFOCUfOGnLCdTn/0zSHoYz89QsEJClImOp787vTK/Md27EtD2eMtujsySinAJsfF6WoDC7V0kcrBKop8WOuPlNE1lCRYYzGyry+fOdZU4Ucl44n+1NGd+JCQRibzgWZJEvkz0rqxhEiMuRKEQD4cfOCxF/RKF4DBQiBMNgGC0ghggRbYOF4AgS5IWI+xGifLVU1qBF2eRgSPJCNFKGZgaDoQVBiKCVg9UGg2GEYITIYDA0HCNEBoOh4RghMhgMDacSIWL6mwC0wWAwVIVKhIikpahlVJkGLDs/wGAwjC4qEaLNZGQyR4E8IvJ0DAaDYRBRhQhviIV1UbdcJq28qolQBoNh5BBViKi8z2K6qJAWbkq9GgyGcLzyMqvZ6+hkGYtBCzOmyzW2vzUYDIZ++GnVonCtGftusRYLmBlj+MVeXyzNYHO3SbJKYEjGgs3b/FsGg8EQgAjlr+Q9IvbNov4LhnhwmfdohmOUwKAUgcFgMPQDIYLCGBH7V7MZHUYFwmptTIfAMWtmMBgMoRQKUcmashVCHRM2/zcYDIaiVJJHFIULZX/NXTUYDIYiaHiWjxGxP/vA+M5wjKLnVDE0GAyGUPwAkaiVR8ROF2z5U68SmAaDoZWRGFXbI3petqvMYDAYSuK7Q6LaQsQeRjvIDAaDYUh8FRLVEiL2/jpXRga2wWAwlIWvQmK4QsT6sVtk+8sMBoMhEr4KiUIhIgN6oNCEGflGTMmzBzxbBFFryGAwGCLjq5AoXGt2mS624j7/QC7HiGUeBJ/flbF1LRsPPiNjLzD2OjIYDIaKQYS4XCNEYs0Vg8FgqCe1zqw2GAyGITFCZDAYGo4RIoPB0HCMEBkMhgZjWf8fs/Umq5JXsrkAAAAASUVORK5CYII=',
                    width: 600,
                    height: 60
                });
                doc.content.alignment = 'center';
                //doc.content.splice(1, 0, {
                //    margin: [0, 0, 0, 0],
                //    alignment: 'right', // Derecha
                //    image: 'data:image/png;base64,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',
                //    width: 270,
                //    height: 45,
                //    position: absolute
                //});

            },

            text: '<i class="fas fa-file-pdf">Pdf</i>',
            title: 'Soporte de Notificacion.', //'Soporte de transito en conexion',
            titleAttr: 'PDF',

            exportOptions: {
                columns: [2, 3, 4, 5, 6, 7, 8, 9, 11]
            }

        }
    ]
});
tableP.column(0).visible(false);
tableP.column(1).visible(false);
tableP.column(2).visible(false);

$("#example2").DataTable();

//cargue tripulantes

function LeerTextoTripulantes(archivo) {
  
    document.getElementById('validacionListadoTri').innerHTML = "";
    if (archivo.files && archivo.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var salida = e.target.result;

            var lineas = salida.split("\n");
            var errores = "";
            var campos;

            lineas.forEach(function (linea, index, array) {

                var final = index + 1;

                if (lineas.length + 1 !== final) {

                    if (linea.length !== 0) {

                        campos = linea.split(",");

                        var amplitud = campos.length;

                        if (amplitud === 3) {
                            var nombreTripulante = validarMaxAmplitud(campos[0], 50);
                            var licenciaTripulante = validarMaxAmplitud(campos[1], 20);
                            var funcionTripulante = validarMaxAmplitud(campos[2], 40);


                            //TipoVuelo
                            if (!nombreTripulante) {
                                errores += ', En la línea ' + (index + 1) + ' el nombre excede la longitud';
                            }
                            if (!licenciaTripulante) {
                                errores += ', En la línea ' + (index + 1) + ' la licencia excede la longitud';
                            }
                            if (!funcionTripulante) {
                                errores += ', En la línea ' + (index + 1) + ' la función de vuelo excede la longitud';
                            }

                        } else {
                            errores = 'Archivo debe tener 3 campos.';
                            $("#btnTripulacion").prop("disabled", true);
                        }

                    }
                }
            });

            if (errores) {
                document.getElementById('validacionListadoTri').innerHTML = '<div class="tool__valide" role="alert"><span> Error en la(s) línea(s) : ' + errores + ' </span></div>';
                this.value = '';
            }

            else {
                document.getElementById('validacionListadoTri').innerHTML = '<div class="tool__valide" role="alert"><span> Archivo validado exitosamente </span></div>';
                $("#btnTripulacion").prop("disabled", false);
            }

        };
        reader.readAsText(archivo.files[0]);
    }


}


function LeerTexto(archivo, tipo) {
        document.getElementById('main').innerHTML = "";
    if (archivo.files && archivo.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var salida = e.target.result;
            var lineas = salida.split("\n");
            var errores = ""; 
            var conteo = 0;
            var campos;
            
            lineas.forEach(function (linea, index, array) {
                
                var final = index + 1
               
                if (lineas.length + 1 != final) {

                    if (linea.length !== 0) {

                        campos = linea.split(",");

                        var amplitud = campos.length;

                        if (amplitud === 14) {
                            var Fecha = validarAmplitud(campos[0], 10);
                            var TipoVuelo = validarAmplitud(campos[1], 3);
                            var Matriculita = validarMaxAmplitud(campos[2], 10);
                            var NumVuelo = validarMaxAmplitud(campos[3], 10);
                            var Destino = validarAmplitud(campos[4], 3);
                            var Hora = validarAmplitud(campos[5], 4);
                            var totalEmbamplitud = validarMaxAmplitud(campos[6], 3);
                            var cantEmbarcados = validarNumero(campos[6]);
                            var TotalEmbarcados = validarNumero(cantEmbarcados);
                            var aINF = validarMaxAmplitud(campos[7], 3);
                            var aTTL = validarMaxAmplitud(campos[8], 3);
                            var aTTC = validarMaxAmplitud(campos[9], 3);
                            var aEX = validarMaxAmplitud(campos[10], 3);
                            var aTRIP = validarMaxAmplitud(campos[11], 3);
                            var aPagoCOP = validarMaxAmplitud(campos[12], 3);
                            var aPagoUSD = validarMaxAmplitud(campos[13], 3);

                            var INF = validarNumero(campos[7]);
                            var TTL = validarNumero(campos[8]);
                            var TTC = validarNumero(campos[9]);
                            var EX = validarNumero(campos[10]);
                            var TRIP = validarNumero(campos[11]);
                            var PagoCOP = validarNumero(campos[12]);
                            var PagoUSD = validarNumero(campos[13]);
                            var lDestino = validarNumero(campos[11]);

                            //Fecha
                            if (!Fecha) {
                                errores += ', En la línea ' + (index + 1) + ' fecha invalida';
                            } else if (!validarFormatoFecha(campos[0])) {
                                errores += ', En la línea ' + (index + 1) + ' fecha invalida';
                            }

                            //Hora
                            if (!Hora) {
                                errores += ', En la línea ' + (index + 1) + ' hora invalida';
                            } else if (!validateHhMm(campos[5])) {
                                errores += ', En la línea ' + (index + 1) + ' hora invalida';
                            }
                            //TipoVuelo
                            if (!TipoVuelo) {
                                errores += ', En la línea ' + (index + 1) + ' tipo de vuelo invalido';
                            } else if (!ValidacionPersonalizada(campos[1], '1')) {
                                errores += ', En la línea ' + (index + 1) + ' Tipo de vuelo inválido';
                            }
                            if (!Matriculita) {
                                errores += ', En la línea ' + (index + 1) + ' la matrícula excede la longitud';
                            }
                            if (!NumVuelo) {
                                errores += ', En la línea ' + (index + 1) + ' el número de vuelo excede la longitud';
                            }
                            if (!totalEmbamplitud) {
                                errores += ', En la línea ' + (index + 1) + ' el total de embarcados excede la longitud';
                            } else if (!TotalEmbarcados) {
                                errores += ', En la línea ' + (index + 1) + ' total embaracados invalido';
                            } else if (!validarNumero(campos[6])) {
                                errores += ', En la línea ' + (index + 1) + ' total embaracados invalido';
                            }
                            if (!aINF) {
                                errores += ', En la línea ' + (index + 1) + ' total de infantes inválido';
                            } else if (!INF) {
                                errores += ', En la línea ' + (index + 1) + ' infante invalido';
                            }
                            if (!aTTL) {
                                errores += ', En la línea ' + (index + 1) + ' Tránsito en línea inválido';
                            } else if (!TTL) {
                                errores += ', En la línea ' + (index + 1) + ' Tránsito en línea invalido';
                            }
                            if (!aTTC) {
                                errores += ', En la línea ' + (index + 1) + ' Tránsito en conexión inválido';
                            } else if (!TTC) {
                                errores += ', En la línea ' + (index + 1) + ' Tránsito en conexión invalido';
                            }
                            if (!aEX) {
                                errores += ', En la línea ' + (index + 1) + ' Excento inválido';
                            } else if (!EX) {
                                errores += ', En la línea ' + (index + 1) + ' Excento invalido';
                            }
                            if (!aTRIP) {
                                errores += ', En la línea ' + (index + 1) + ' tripulación inválida';
                            } else if (!TRIP) {
                                errores += ', En la línea ' + (index + 1) + ' tripulacion invalida';
                            }
                            if (!aPagoCOP) {
                                errores += ', En la línea ' + (index + 1) + ' Pago en COP inválido';
                            } else if (!PagoCOP) {
                                errores += ', En la línea ' + (index + 1) + ' Pago en COP inválido';
                            }
                            if (!aPagoUSD) {
                                errores += ', En la línea ' + (index + 1) + ' Pago en USD inválido';
                            } else if (!PagoUSD) {
                                errores += ', En la línea ' + (index + 1) + ' Pago en USD inválido';
                            }

                            //Destino
                            if (!Destino) {
                                errores += ', En la línea ' + (index + 1) + ' destino invalido';
                            } else if (!ValidacionPersonalizada(campos[4], '1')) {
                                errores += ', En la línea ' + (index + 1) + ' destino invalido';
                            }
                            conteo = conteo + 1;

                        } else {
                            errores = 'Archivo debe tener 14 campos.';
                        }
                    }
                }
            }
                );
            if (errores) {
                document.getElementById('main').innerHTML = '<div class="tool__valide" role="alert"><span> Error en la(s) línea(s) : ' + errores + ' </span></div>';
            }

            else {
                var cantRegistro = conteo;

                $("#cantRegistroLlegada").val(cantRegistro);
                $("#cantRegistroSalida").val(cantRegistro);

                document.getElementById('main').innerHTML = '<div class="tool__valide" role="alert"><span> Archivo validado exitosamente </span></div>';

                if (tipo == "LLEGADA") {
                    $("#btnCargarLlegada").show();
                }
                else if (tipo == "SALIDA") {
                    $("#btnCargarSalida").show();
                }
                else {
                    $("#btnCargarLlegada").hide();
                    $("#btnCargarSalida").hide();
                }
            }

        };
        reader.readAsText(archivo.files[0]);
    }
}

function ValidarContingenciaVuelos(archivo) {
    // validaciones previas carga de contingencia
    document.getElementById('main').innerHTML = "";

    if (archivo.files && archivo.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var salida = e.target.result;

            var lineas = salida.split("\n");
            var errores = "";
            var campos;

            lineas.forEach(function (linea, index, array) {

                var final = index + 1;

                if (lineas.length !== final) {
                    if (linea.length !== 0) {

                        campos = linea.split(",");
                   
                    var amplitud = campos.length;

                    if (amplitud === 10) {
                        // aca van las validaciones de estructura del archivo csv contingencia de JARVIS
                        var Id_Daily = validarNumero(campos[0]);

                        if (!Id_Daily) {
                            errores += ', En la línea ' + (index + 1) + ' Id_Daily número invalido';
                        }

                        var TipoVuelo = validarAmplitudEx(campos[1], 3);

                        if (!TipoVuelo) {
                            errores += ', En la línea ' + (index + 1) + ' tipoVuelo invalido';
                        } else {
                            if (campos[1] != "INT" && campos[1] != "DOM") {
                                errores += ', En la línea ' + (index + 1) + ' TipoVuelo invalido';
                            }
                        }

                        // aca van las validaciones de estructura del archivo csv contingencia de JARVIS
                        var Id_Vuelo = validarNumero(campos[2]);

                        if (!Id_Vuelo) {
                            errores += ', En la línea ' + (index + 1) + ' Id_Vuelo número invalido';
                        }

                        var MatriculaVuelo = validarAmplitudEx(campos[3], 8);

                        if (!MatriculaVuelo) {
                            errores += ', En la línea ' + (index + 1) + ' MatriculaVuelo invalida';
                        }

                        var NumVuelo = validarAmplitudEx(campos[4], 6);

                        if (!NumVuelo) {
                            errores += ', En la línea ' + (index + 1) + ' NumVuelo invalido';
                        }

                        var IdAerolinea = validarAmplitudEx(campos[5], 3);

                        if (!IdAerolinea) {
                            errores += ', En la línea ' + (index + 1) + ' IdAerolinea invalido';
                        }

                        var Destino = validarAmplitudEx(campos[6], 3);

                        if (!Destino) {
                            errores += ', En la línea ' + (index + 1) + ' Destino invalido';
                        }

                        var Fecha = validarAmplitudEx(campos[7], 10);

                        if (!Fecha) {
                            errores += ', En la línea ' + (index + 1) + ' fecha invalida';
                        } else if (!validarFormatoFecha(campos[7])) {
                            errores += ', En la línea ' + (index + 1) + ' Fecha invalida';
                        } else {

                            // prepare fecha actual para comparar...vs archivo plano csv
                            var hoy = new Date();
                            var day = hoy.getDate();
                            var day1 = day;
                            if (day <= 9) {
                                day1 = '0' + day;
                            }
                            var month = hoy.getMonth() + 1;
                            var monthFn = hoy.getMonth() + 1;
                            if (month <= 9) {
                                monthFn = '0' + month;
                            }
                            var year = hoy.getFullYear();
                            var fchConcat = day1 + '/' + monthFn + '/' + year;

                            // prepare fecha archivo plano csv
                            var fchArch = campos[7].split("/");

                            var fechaArch2 = new Date(fchArch[2], fchArch[1], fchArch[0]);
                            var day2 = fechaArch2.getDate();
                            var dayFn = day2;
                            if (day2 <= 9) {
                                dayFn = '0' + day;
                            }
                            var month2 = fechaArch2.getMonth() + 1;
                            var monthFn2 = fechaArch2.getMonth() + 1;
                            if (month2 <= 9) {
                                monthFn2 = '0' + month2;
                            }
                            var year2 = fechaArch2.getFullYear();
                            var fechaArchFn = dayFn + '/' + monthFn2 + '/' + year2;

                            //console.log("fechaArchFn: " + fechaArchFn);
                            //console.log("fchConcat: " + fchConcat);

                            // comparacion de fecha actual vs fecha archivo plano segun cada línea del file csv
                            if (fechaArchFn > fchConcat) {
                                errores += ', En la línea ' + (index + 1) + ' Fecha invalida mayor a la actual';
                            }
                        }

                        var Hora = validarAmplitudEx(campos[8], 4);

                        //Hora
                        if (!Hora) {
                            errores += ', En la línea ' + (index + 1) + ' hora invalida';
                        } else if (!validateHhMm(campos[8])) {
                            errores += ', En la línea ' + (index + 1) + ' Hora invalida';
                        }

                        var Rmk = validarAmplitudEx(campos[9], 3);
                        // valor sin salto de línea... pra validar contenido segun rule de negocio...
                        var RmkFn = remove_linebreaks(campos[9]);

                        if (!Rmk) {
                            errores += ', En la línea ' + (index + 1) + ' Rmk invalido';
                        } else if (RmkFn == "DLY" || RmkFn == "CAN" || RmkFn == "BOR") {
                            errores += ', En la línea ' + (index + 1) + ' Rmk invalido no debe ser "DLY", "CAN" o "BOR"';
                        }

                    } else {
                        errores = 'Archivo debe tener 10 campos.';
                    }
                }
            }
            });

            if (errores) {
                document.getElementById('main').innerHTML = '<div class="tool__valide" role="alert"><span> Error en la(s) línea(s) : ' + errores + ' </span></div>';
                this.value = '';
            } else {
                document.getElementById('main').innerHTML = '<div class="tool__valide" role="alert"><span> Archivo validado exitosamente </span></div>';
                $("#btnCargarLlegada").show();
            }

        };
        reader.readAsText(archivo.files[0]);
    }

}

function ValidarArchivoPasajeros(archivo) {
    
    document.getElementById('validacionListado').innerHTML = "";
    if (archivo.files && archivo.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var salida = e.target.result;

            var lineas = salida.split("\n");
            var errores = "";
            var campos;

            lineas.forEach(function (linea, index, array) {
                
                var final = index + 1;

                if (lineas.length + 1 !== final) {

                    if (linea.length !== 0) {

                        campos = linea.split(",");
                   
                    var amplitud = campos.length;
                        
                    if (amplitud === 5) {
                        var Fecha = validarAmplitud(campos[0], 10);
                        var NumeroVuel = validarMaxAmplitud(campos[1], 10);
                        var NumeroMat = validarMaxAmplitud(campos[2], 10);
                        var NombrePaxAm = validarMaxAmplitud(campos[3], 50);
                        var NombrePax = ValidacionPersonalizada(campos[3], 1);
                        var TipoPax = ValidacionPersonalizada(campos[4], 1);
                        var Categ = validarMaxAmplitud(campos[4], 4);
                        var cateVacia = validarCampoVacio(campos[4]);
                        
                        //Fecha
                        if (!Fecha) {
                            errores += ', En la línea ' + (index + 1) + ' fecha invalida';
                        } else if (!validarFormatoFecha(campos[0])) {
                            errores += ', En la línea ' + (index + 1) + ' fecha invalida';
                        }
                        if (!NombrePax) {
                            errores += ', En la línea ' + (index + 1) + ' el nombre no debe contener números';
                        } else if (!NombrePaxAm) {
                            errores += ', En la línea ' + (index + 1) + ' el nombre excede la longitud permitida';
                        }
                        if (!TipoPax) {
                            errores += ', En la línea ' + (index + 1) + ' la categoría no debe contener números';
                        } else if (!Categ) {
                            errores += ', En la línea ' + (index + 1) + ' la categoría excede la longitud permitida';
                        } else if (!cateVacia) {
                            errores += ', En la línea ' + (index + 1) + ' la categoría se encuentra vacia';
                        }


                        if (!NumeroVuel) {
                            errores += ', En la línea ' + (index + 1) + ' el número de vuelo excede la longitud';
                        }
                        if (!NumeroMat) {
                            errores += ', En la línea ' + (index + 1) + ' el número de matrícula excede la longitud';
                        }

                    } else {
                        errores = 'Archivo debe tener 5 campos.';
                        $("#btnCargarPasajeros").prop("disabled", true);
                    }

                }
                }
            });

            if (errores) {
                document.getElementById('validacionListado').innerHTML = '<div class="tool__valide" role="alert"><span> Error en la(s) línea(s) : ' + errores + ' </span></div>';
                this.value = '';
            }

            else {
                document.getElementById('validacionListado').innerHTML = '<div class="tool__valide" role="alert"><span> Archivo validado exitosamente </span></div>';
                $("#btnCargarPasajeros").prop("disabled", false);
            }

        };
        reader.readAsText(archivo.files[0]);
    }



}

function validarFormatoFecha(campo) {
    var RegExPattern = /^\d{1,2}\/\d{1,2}\/\d{2,4}$/;
    if ((campo.match(RegExPattern)) && (campo != '')) {
        if (campo.substring(3,5) > 12) {
            return false;
        }
        return true;
    } else {
        return false;
    }
}


function validarCampoVacio(campo)
{
   
    campo = campo.replace("&nbsp;", "");
    campo = campo == undefined ? "" : campo;
    if (!campo || 0 === campo.trim().length) {
        return false;
    }
    else {
        return true;
    }

}

function ValidarArchivoTransitos(archivo) {
    document.getElementById('validacionListadoTransitos').innerHTML = "";    
    
    if (archivo.files && archivo.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            var salida = e.target.result;

            var lineas = salida.split("\n");
            var errores = "";
            var campos;

            lineas.forEach(function (linea, index, array) {
                var final = index + 1;

                if (lineas.length + 1 !== final) {

                    if (linea.length !== 0) {

                        campos = linea.split(",");
                   

                    var amplitud = campos.length;

                    if (amplitud === 11) {
                        var FechaLlegada = validarAmplitud(campos[0], 10);
                        var HoraLlegada = validarAmplitud(campos[1], 4);
                        var FechaSalida = validarAmplitud(campos[4], 10);
                        var HoraSalida = validarAmplitud(campos[5], 4);
                        var TTC = validarNumero(campos[9]);
                        var TTL = validarNumero(campos[10]);
                        var lonvllegada = validarMaxAmplitud(campos[2], 10);
                        var lonvsalida = validarMaxAmplitud(campos[6], 10);
                        var lonorigen = validarMaxAmplitud(campos[3], 3);
                        var lonllegada = validarMaxAmplitud(campos[7], 3);

                        var origen = ValidacionPersonalizada(campos[3], 1);
                        var llegada = ValidacionPersonalizada(campos[7], 1);
                        var lonnom = validarMaxAmplitud(campos[8], 50);
                        var nombre = ValidacionPersonalizada(campos[8], 1);
                        var lonTTC = validarMaxAmplitud(campos[9], 1);

                        
                        var lonTTL = validarMaxAmplitud(remove_linebreaks(campos[10]), 1);

                        //validacion origen y destino en transito
                        var maxMinOrigen = validacionPersonalizadaTransito(campos[3], 3);
                        var maxMinLlegada = validacionPersonalizadaTransito(campos[7], 3);


                        if (!FechaLlegada) {
                            errores += ', En la línea : ' + (index + 1) + ' la Fecha llegada no es válida, ';
                        } else if (!validarFormatoFecha(campos[0])) {
                            errores += ', En la línea : ' + (index + 1) + ' la fecha llegada invalida';
                        }

                        if (!FechaSalida) {
                            errores += ', En la línea : ' + (index + 1) + ' la Fecha Salida no es válida, ';
                        } else if (!validarFormatoFecha(campos[4])) {
                            errores += ', En la línea : ' + (index + 1) + ' la fecha salida invalida';
                        }

                        if (!HoraLlegada) {
                            errores += ', En la línea : ' + (index + 1) + ' la Hora Llegada no es válida, ';
                        } else if (!validateHhMm(campos[1])) {
                            errores += ', En la línea : ' + (index + 1) + ' la Hora Llegada no es válida, ';
                        }

                        if (!HoraSalida) {
                            errores += ', En la línea : ' + (index + 1) + ' la Hora Salida no es válida, ';
                        } else if (!validateHhMm(campos[5])) {
                            errores += ', En la línea : ' + (index + 1) + ' la Hora Salida no es válida, ';
                        }
                        if (!TTC) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo TTC(Tránsito Conexiòn) debe ser numérico, ';
                        } else if (!lonTTC) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo TTC excede la longitud, ';
                        } else if (campos[9] != 1 && campos[9] != 0) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo TTC debe ser 1 o 0, ';
                        }

                        if (!TTL) {
                            errores += ', En la línea : ' + (index + 1) + ' El campo TTL(Tránsito en Lìnea) debe ser númerico.';
                        } else if (!lonTTL) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo TTL excede la longitud, ';
                        } else if (campos[10] != 1 && campos[10] != 0) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo TTL debe ser 1 o 0, ';
                        }
                        if (!nombre) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo nombre no debe ser numérico, ';
                        } else if (!lonnom) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo nombre excede la longitud, ';
                        }
                        if (!origen) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo origen o destino no debe ser numérico, ';
                        } else if (!lonorigen) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo origen o destino excede la longitud, ';
                        } else if (!maxMinOrigen) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo origen o destino no cuenta con la longitud permitida, ';
                        }

                        if (!llegada) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo origen o destino no debe ser numérico, ';
                        } else if (!lonllegada) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo origen o destino excede la longitud, ';
                        } else if (!maxMinLlegada) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo origen o destino no cuenta con la longitud permitida, ';
                        }

                        if (!lonvllegada) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo Vuelo llegada excede la longitud, ';
                        }
                        if (!lonvsalida) {
                            errores += ', En la línea : ' + (index + 1) + ' el campo Vuelo salida excede la longitud, ';
                        }


                    }
                    else {
                        errores = 'Archivo debe tener 11 campos.';
                        $("#btnCargarTransitos").prop("disabled", true);
                    }
                }  }
            });

            if (errores) {
                document.getElementById('validacionListadoTransitos').innerHTML = '<div class="tool__valide" role="alert"><span> Error en la(s) línea(s) : ' + errores + ' </span></div>';
                this.value = '';
            }

            else {

                document.getElementById('validacionListadoTransitos').innerHTML = '<div class="tool__valide" role="alert"><span> Archivo validado exitosamente </span></div>';

                $("#btnCargarTransitos").prop("disabled", false);
            }

        };
        reader.readAsText(archivo.files[0]);
    }
}

function validateHhMm(inputField) {
    
    var str = inputField;
    var patt1 = /^(0[0-9]|1[0-9]|2[0-3])[0-5][0-9]$/gm;
    var result = str.match(patt1);

    return result;
}

function validarNumero(numero) {
    var valor = isNaN(numero);

    if (valor === true)
        return false;
    else
        return true;
}
 

function validarAmplitud(cadena, amplitud) {

    //campos vacios
    if (cadena.length < 1) {
        return false;
    }

    if (cadena.length === amplitud) {
        return true;
    }

    else {
        var cadenaFn = remove_linebreaks(cadena);

        if (cadenaFn.length == amplitud)
            return true;
        else
            return false;        
    }
}
function validarMaxAmplitud(cadena, amplitud) {

    

    if (cadena.length < 1) {
        return false;
    }


    if (cadena.length <= amplitud) {
        return true;
    }
    else {
        var cadenaFn = remove_linebreaks(cadena);

        if (cadenaFn.length == amplitud)
            return true;
        else
            return false;
    }
}
function ValidacionPersonalizada(campo,tipo) {
    if (tipo = '1') {
        // este es el campo fecha formato solo letras
        if (campo.includes('1') || campo.includes('2') || campo.includes('3') || campo.includes('4') || campo.includes('5') || campo.includes('5') || campo.includes('7') || campo.includes('8') || campo.includes('9') || campo.includes('0')) {
            return false;
        } else { return true;}

    }
}


function validacionPersonalizadaTransito(cadena, amplitud) {

    if (cadena.length === 3) {
        return true;
    }
    else {
       
        return false;
    }
}

function remove_linebreaks( valor ) {
    return valor.replace(/[\r\n]+/gm, "");
}

function validarAmplitudEx(cadena, amplitud) {

    if (cadena.length <= amplitud) {
        if (cadena.length == 0)
            return false;
        else
            return true;
    }
    else {
        var cadenaFn = remove_linebreaks(cadena);

        if (cadenaFn.length <= amplitud) {
            if (cadenaFn.length == 0)
                return false;
            else
                return true;
        }else
            return false;
    }
}

function CargarVistaVuelo(id, accion) {
    $("#divAcciones").empty();
    $("#divAcciones2").empty();
    $("#divAcciones").load("/CargaInformacion/Vuelos/" + accion + "/" + id);
}

function CargarArchivo(id, tipo) {
    $("#divMensaje").empty();
    $("#divAcciones2").empty();
    $("#divAcciones").load("/CargaInformacion/Archivos/Cargar?idOperacion=" + id + "&tipo=" + tipo);
}

function CargarListado(id, tipo) {
    $("#divMensaje").empty();
    $("#divAcciones").empty();
    $("#divMensaje").empty();
    $("#divAcciones2").empty();
    if (tipo == "PASAJEROS")
        $("#divAcciones").load("/CargaInformacion/Pasajeros/Cargar?idOperacion=" + id);

    if (tipo == "TRANSITOS")
        $("#divAcciones").load("/CargaInformacion/Transitos/Cargar?idOperacion=" + id);
}

function VerListado(id, tipo, accion) {
    $("#divAcciones").empty();
    $("#divMensaje").empty();
    $("#divAcciones2").empty();
    if (tipo == "PASAJEROS")
        $("#divAcciones").load("/CargaInformacion/Pasajeros/Principal?idOperacion=" + id + "&acciones=" + accion);

    if (tipo == "TRANSITOS")
        $("#divAcciones").load("/CargaInformacion/Transitos/Principal?idOperacion=" + id + "&acciones=" + accion);
}

function InsertarPasajero(id) {
    $("#divMensaje").empty();
    $("#divAcciones").load("/CargaInformacion/Pasajeros/Insertar?idOperacion=" + id);
}

function InsertarTransito(id) {
    $("#divMensaje").empty();
    $("#divAcciones").load("/CargaInformacion/Transitos/Insertar?idOperacion=" + id);
}

function ActualizarPasajero(id) {
    $("#divMensaje").empty();
    $("#divAcciones").load("/CargaInformacion/Pasajeros/Actualizar/" + id);
}
function ActualizarRealizaViajeEx(id, dato,idjavis) {
    var checkeado;
    if (dato.checked) {
        checkeado = '1';
    } else {
        checkeado = '0';
    }
        $.ajax({
            type: 'POST',
            url: "/CargaInformacion/Pasajeros/GuardarViajeExtento",
            data:
            {
                idExtento: id,
                viaja: checkeado,
                idpasajeroJ: idjavis
            },
            success: function (data) {
                $("#divMensaje").empty();
                if (data) {
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-warning fade show" role="alert">Actualizado correctamente <strong>Exitosamente. </strong></div>';
                }
                else {
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al actualizar pasajero.</div>';
                }
            },
            error: function (data) {
                $("#divMensaje").empty();
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al guardar pasajero.</div>';
            }
        })
    
    
}

function ActualizarExento(id) {
    $("#divMensaje").empty();
    $("#divCargarEX").load("/CargaInformacion/Pasajeros/ActualizarEx/" + id);
}

function ActualizarTransito(id) {
    $("#divMensaje").empty();
    if ($("#HoraLlegada").val() == "" || $("#NumeroVueloLlegada").val() == "" || $("#Origen").val() == "" || $("#NumeroVueloSalida").val() == "" || $("#HoraSalida").val() == "" || $("#DestinoTransito").val() == "" || $("#NombrePasajero").val() == "") {
        document.getElementById('divMensajeAlert').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> Debe completar los campos.</div>';
    } else {
        $("#divAcciones").load("/CargaInformacion/Transitos/Actualizar/" + id);
    }
   
}

function InsertarPasajeroPost(id) {
    if ($("#fechaPasajero").val() == "" || $("#numeroVuelo").val() == "" || $("#matriculaVuelo").val() == "" || $("#nombrePasajero").val() == "" || $("#categoria").val() == "") {
        document.getElementById('divMensajePasAlert').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> Debe completar los campos.</div>';
    }
    else {
        document.getElementById('divMensajePasAlert').innerHTML = '';
        $.ajax({
            type: 'POST',
            url: "/CargaInformacion/Pasajeros/Insertar",
            data:
            {
                pasajeroOtd:
                {
                    Fecha: $("#fechaPasajero").val(),
                    NumeroVuelo: $("#numeroVuelo").val(),
                    MatriculaVuelo: $("#matriculaVuelo").val(),
                    Operacion: parseInt($("#operacionPasajero").val()),
                    NombrePasajero: $("#nombrePasajero").val(),
                    Categoria: $("#categoria").val(),
                    realiza_viaje: $("#ddlRealViaje").val(),
                    motivo_exencion: $("#txtMotivoEx").val()
                }
            },
            success: function (data) {

                $("#divMensaje").empty();
                if (data) {
                    $("#divAcciones").load("/CargaInformacion/Pasajeros/Principal?idOperacion=" + id);
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-warning fade show" role="alert">Pasajero guardado <strong>Exitosamente. </strong></div>';
                }
                else {
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al guardar pasajero.</div>';
                }
            },
            error: function (data) {
                $("#divMensaje").empty();
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al guardar pasajero.</div>';
            }
        })

    }

}

function InsertarTransitoPost(id) {
    // verifico que haya llenado el formulario por lo menos los datos obligatorios
    if ($("#HoraLlegada").val() == "" || $("#NumeroVueloLlegada").val() == "" || $("#Origen").val() == "" || $("#NumeroVueloSalida").val() == "" || $("#HoraSalida").val() == "" || $("#DestinoTransito").val() == "" || $("#NombrePasajero").val() == "" ) {
        document.getElementById('divMensajeAlert').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> Debe completar los campos.</div>';
    } else {
        document.getElementById('divMensajeAlert').innerHTML = '';
        $.ajax({
            type: 'POST',
            url: "/CargaInformacion/Transitos/Insertar",
            data:
            {
                pasajeroTransito:
                {
                    Operacion: parseInt($("#Operacion").val()),
                    FechaLlegada: $("#FechaLlegada").val(),
                    HoraLlegada: $("#HoraLlegada").val(),
                    NumeroVueloLlegada: $("#NumeroVueloLlegada").val(),
                    Origen: $("#Origen").val(),
                    FechaSalida: $("#FechaSalida").val(),
                    HoraSalida: $("#HoraSalida").val(),
                    NumeroVueloSalida: $("#NumeroVueloSalida").val(),
                    Destino: $("#DestinoTransito").val(),
                    NombrePasajero: $("#NombrePasajero").val(),
                    TTC: $("#TTC").val(),
                    TTL: $("#TTL").val(),
                    AerolineaLlegada: $("#NumeroVueloLlegada").val().substring(0, 3),
                },
                categoria: $("#Categoria").val()
            },
            success: function (data) {

                $("#divMensaje").empty();
                if (data) {
                    $("#divAcciones").load("/CargaInformacion/Transitos/Principal?idOperacion=" + id);
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-warning alert-dismissible fade show" role="alert">Tránsito guardado <strong>Exitosamente. </strong></div>';
                }
                else {
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al guardar tránsito.</div>';
                }
            },
            error: function (data) {
                $("#divMensaje").empty();
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al guardar pasajero.</div>';
            }
        })

    }
  

}

function ActualizarPasajeroPost(id) {
    $.ajax({
        type: 'POST',
        url: "/CargaInformacion/Pasajeros/Actualizar",
        data:
        {
            pasajeroOtd:
            {
                Id: parseInt($("#idPasajero").val()),
                Fecha: $("#fechaPasajero").val(),
                NumeroVuelo: $("#numeroVuelo").val(),
                MatriculaVuelo: $("#matriculaVuelo").val(),
                Operacion: parseInt($("#operacionPasajero").val()),
                NombrePasajero: $("#nombrePasajero").val(),
                Categoria: $("#categoria").val(),
                realiza_viaje: $("#ddlRealViaje").val(),
                motivo_exencion: $("#txtMotivoEx").val()
            }
        },
        success: function (data) {

            $("#divMensaje").empty();
            if (data) {
                $("#divAcciones").load("/CargaInformacion/Pasajeros/Principal?idOperacion=" + id);
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-warning fade show" role="alert">Pasajero actualizado <strong>Exitosamente. </strong></div>';
            }
            else {
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al actualizar pasajero.</div>';
            }
        },
        error: function (data) {

            //$("#divAcciones").load("/CargaInformacion/Pasajeros/Principal?idOperacion=" + id);
            $("#divMensaje").empty();
            document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al actualizar pasajero.</div>';
        }
    })

}

//ActualizarExentoPost

function ActualizarExentoPost(id) {
    $.ajax({
        type: 'POST',
        url: "/CargaInformacion/Pasajeros/Actualizar",
        data:
        {
            pasajeroOtd:
            {
                Id: parseInt($("#idPasajero").val()),
                Fecha: $("#fechaPasajero").val(),
                NumeroVuelo: $("#numeroVuelo").val(),
                MatriculaVuelo: $("#matriculaVuelo").val(),
                Operacion: parseInt($("#operacionPasajero").val()),
                NombrePasajero: $("#nombrePasajero").val(),
                Categoria: $("#categoria").val(),
                realiza_viaje: $("#ddlRealViaje").val(),
                motivo_exencion: $("#txtMotivoEx").val()
            }
        },
        success: function (data) {

            $("#divMensaje").empty();
            if (data) {
                $("#divCargarEX").empty();
                $("#divCargarEX").load("/CargaInformacion/Pasajeros/Principal?idOperacion=" + id + "&acciones=" + 2)
                setTimeout(function () {
                    var $ex = $("#divCargarEX").find(".imgEditarEx").toArray().length;                    
                    $("#UpdEX").val($ex);
                }, 1000);
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-warning fade show" role="alert">Pasajero actualizado <strong>Exitosamente. </strong></div>';
            }
            else {
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al actualizar pasajero.</div>';
            }
        },
        error: function (data) {

            //$("#divAcciones").load("/CargaInformacion/Pasajeros/Principal?idOperacion=" + id);
            $("#divMensaje").empty();
            document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al actualizar pasajero.</div>';
        }
    })

}

//function UnMomento(url) {
//    window.location.href = url;
//}
function ActualizarTransitoPost(id) {
    var Seleccion =$('input[name="pdfyTxt"]:checked').val();
    if ($("#HoraLlegada").val() == "" || $("#NumeroVueloLlegada").val() == "" || $("#Origen").val() == "" || $("#NumeroVueloSalida").val() == "" || $("#HoraSalida").val() == "" || $("#DestinoTransito").val() == "" || $("#NombrePasajero").val() == "") {
        document.getElementById('divMensajeAlert').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> Debe completar los campos.</div>';
    } else {

        $.ajax({
            type: 'POST',
            url: "/CargaInformacion/Transitos/Actualizar",
            data:
            {
                pasajeroTransito:
                {
                    Id: parseInt($("#Id").val()),
                    Operacion: parseInt($("#Operacion").val()),
                    FechaLlegada: $("#FechaLlegada").val(),
                    HoraLlegada: $("#HoraLlegada").val(),
                    NumeroVueloLlegada: $("#NumeroVueloLlegada").val(),
                    Origen: $("#Origen").val(),
                    FechaSalida: $("#FechaSalida").val(),
                    HoraSalida: $("#HoraSalida").val(),
                    NumeroVueloSalida: $("#NumeroVueloSalida").val(),
                    Destino: $("#DestinoTransito").val(),
                    NombrePasajero: $("#NombrePasajero").val(),
                    TTC: $("#TTC").val(),
                    TTL: $("#TTL").val(),
                    FechaHoraCargue: $("#FechaHoraCargue").val(),
                    AerolineaLlegada: $("#NumeroVueloLlegada").val().substring(0, 3),
                },
                categoria: $("#Categoria").val(),
                pdfyTxt: Seleccion
            },
            success: function (data) {

                $("#divMensaje").empty();
                if (data) {
                    $("#divAcciones").load("/CargaInformacion/Transitos/Principal?idOperacion=" + id + "&retornopdfotxt=" + data.nombreArchivo);
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-warning fade show" role="alert">Tránsito actualizado <strong>Exitosamente. </strong></div>';
                }
                else {
                    document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al actualizar tránsito.</div>';
                }
            },
            error: function (data) {

                //$("#divAcciones").load("/CargaInformacion/Transitos/Principal?idOperacion=" + id);
                $("#divMensaje").empty();
                document.getElementById('divMensaje').innerHTML = '<div class="alert alert-dark fade show" role="alert"><strong>Error </strong> al actualizar tránsito.</div>';
            }
        })
    }

  

}

function VerArchivoNuevo(carpeta, nombre) {   
    $("#divAccionesMary").html("<h1>" + carpeta + "</h1><object type='application/pdf' data='/CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "' style='width: 100%; height: 73vh'>Psor favor de click <a href=data='//CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "'> <u> aquí para descargar el pdf</u></a></object>");
}

function Suspend(ID) {
    $("#p-warning-content").text("¿Esta seguro que desea realizar la suspención del la operación de vuelo seleccionada?.");
    $("#divMensaje").empty();
    $("#divAcciones").empty();
    $("#divAcciones2").empty();
    $("#BodyMaster").toggle();
    $("#modal-warning").show();
}
function CloseSuspend() {
    try {
        $("#divMensaje").empty();
        $("#divAcciones").empty();
        $("#divAcciones2").empty();
        $("#BodyMaster").show();
        $("#modal-warning").toggle();
    }
    catch (o) {

    }
}

function limpiarCache() {
    if ('caches' in window) {
        caches.keys().then(function (cacheNames) {
            cacheNames.forEach(function (cacheName) {
                caches.delete(cacheName);
            });
        });
    }

    // Limpiar caché del almacenamiento local (localStorage)
    localStorage.clear();

    // Limpiar caché del almacenamiento de sesión (sessionStorage)
    sessionStorage.clear();

    // Recargar la página
    //location.reload(true);
}


function VerArchivo(carpeta, nombre) {

    limpiarCache();
    // se limpia la modal para trabajar....
    var res = nombre.split("//");
    var filename = nombre;    
    if (res[2] != null) {
        filename = res[2];
    }
    $("#divMensaje").empty();
    $("#divAcciones").empty();
    $("#divAcciones2").empty();
    // valido si es archivos o pdf's segun la ruta del archivo relacionado
    if (carpeta != 'ARCHIVOS') {
        $("#divAcciones").html("<h1>" + filename + "</h1><object type='application/pdf' data='/CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "' style='width: 100%; height: 73vh'>El navegador nop soporta la vista previa, por favor de click <a href=data='//CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "'> <u> aquí para descargar el pdf</u></a></object>");
       
        
    }
    else {

        //se recupera del servidor el csv o txt
        var TXT_URL = "/CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "";
        // se usa ajax para procesar y mostrar en pagina...
        $.ajax
            (
                {
                    url: TXT_URL,
                    dataType: "text",
                    success: function (data) {
                        $("#divAcciones").html("<pre>" + data + "</pre>");
                    }
                }
            );
    }
}

function Apply_Suspend()
{
    alert("Suspención aplicada");
}

function VerArchivo2(carpeta, nombre, id) {
    //debugger;
    // se limpia la modal para trabajar....
    $("#divMensaje").empty();
    $("#divAcciones").empty();
    $("#divAcciones2").empty();
    // valido si es archivos o pdf's segun la ruta del archivo relacionado
    if (carpeta != 'ARCHIVOS') {
        $("#divAcciones").html("<h1>" + carpeta + "</h1><object type='application/pdf' data='/CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "' style='width: 100%; height: 73vh'>El navegador nop soporta la vista previa, por favor de click <a href=data='//CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "'> <u> aquí para descargar el pdf</u></a></object>");
        // cargo en el otro div los manuales
        $("#divAcciones2").load("/CargaInformacion/Pasajeros/Principal?idOperacion=" + id + "&acciones=1");
    }
    else {

        //se recupera del servidor el csv o txt
        var TXT_URL = "/CargaInformacion/Archivos/ObtenerArchivo?carpeta=" + carpeta + "&nombreArchivo=" + nombre + "";
        // se usa ajax para procesar y mostrar en pagina...
        $.ajax
            (
                {
                    url: TXT_URL,
                    dataType: "text",
                    success: function (data) {
                        $("#divAcciones").html("<pre>" + data + "</pre>");
                    }
                }
        );

     
    }

}

function LlegadaTransito(id) {
    $("#divMensaje").empty();
    $("#divAcciones").load("/TransitoConexion/TransitoConexion/Actualizar/" + id);
}

function LlegadaTransitoOnBlur(id, fecha, hora) {

    $.ajax({
        type: 'GET',
        url: "/TransitoConexion/TransitoConexion/ActualizarOnBlur?id=" + id + "&fechaLlegada=" + fecha + "&horaLlegada=" + hora,
        success: function (data) {
            //$("#mensajeGuardado").empty();
            if (data) {
                console.log('Los datos fueron guardados correctamente');
            }
            else {
                console.log('Error al guardar los datos.');
            }
        },
        error: function (data) {
            //$("#mensajeGuardado").empty();
            console.log('Error al guardar los datos.');
        }
    })

}
function AfterClick() {
    if ($("#mensajeGuardado").html() == "<p>Las tasas fueron guardadas correctamente</p>") {
        location.reload();
    }
   
}
function GuardarTasaPost() {
    $.ajax({
        type: 'POST',
        url: "/Administracion/Tasa/Insertar",
        data:
        {
            tasaOtd:
            {
                ValorCOP: parseInt($("#ValorCOP").val()),
                ValorUSD: parseInt($("#ValorUSD").val())
            }
        },
        success: function (data) {
            $("#mensajeGuardado").empty();
            if (data) {
                document.getElementById('mensajeGuardado').innerHTML = '<p>Las tasas fueron guardadas correctamente</p>';
            }
            else {
                document.getElementById('mensajeGuardado').innerHTML = '<p>Error al guardar los datos.</p>';
            }
        },
        error: function (data) {
            $("#mensajeGuardado").empty();
            document.getElementById('mensajeGuardado').innerHTML = '<p>Error al guardar los datos.</p>';
        }
    })

}

function GuardarHorarioOperacionPost() {
    $.ajax({
        type: 'POST',
        url: "/Administracion/HorarioOperacion/Insertar",
        data:
        {
            horarioOperacionOtd:
            {
                Dia: $("#DiaOperacion").val(),
                HoraInicio: $("#HoraInicioOperacion").val(),
                HoraFin: $("#HoraFinOperacion").val()
            }
        },
        success: function (data) {
            $("#mensajeGuardado").empty();
            if (data) {
                document.getElementById('mensajeGuardado').innerHTML = '<p>Los datos fueron guardados correctamente</p>';
                $("#divHorarioOperacion").empty();
                $("#divHorarioOperacion").load("/Administracion/HorarioOperacion/Principal");
            }
            else {
                document.getElementById('mensajeGuardado').innerHTML = '<p>Error al guardar los datos.</p>';
            }
        },
        error: function (data) {
            $("#mensajeGuardado").empty();
            document.getElementById('mensajeGuardado').innerHTML = '<p>Error al guardar los datos.</p>';
        }
    })
}

function ListarHorarioOperacion() {
    $("#divHorarioOperacion").empty();
    $("#divHorarioOperacion").load("/Administracion/HorarioOperacion/Principal");
}

function GuardarHorarioAerolineaPost() {
    $.ajax({
        type: 'POST',
        url: "/Administracion/HorariosAerolineas/Insertar",
        data:
        {
            horarioAerolineaOtd:
            {
                IdAerolinea: $("#IdAerolinea").val(),
                HoraInicio: $("#HoraInicio").val(),
                HoraFin: $("#HoraFin").val()
            }
        },
        success: function (data) {
            $("#mensajeGuardado").empty();
            if (data) {
                document.getElementById('mensajeGuardado').innerHTML = '<p>Los datos fueron guardados correctamente</p>';
            }
            else {
                document.getElementById('mensajeGuardado').innerHTML = '<p>Error al guardar los datos, puede estar duplicado.</p>';
            }
        },
        error: function (data) {
            $("#mensajeGuardado").empty();
            document.getElementById('mensajeGuardado').innerHTML = '<p>Error al guardar los datos.</p>';
        }
    })

}

function ListarHorarioAerolinea() {
    $("#divHorarioAerolinea").empty();
    $("#divHorarioAerolinea").load("/Administracion/HorariosAerolineas/Principal");
}

function EliminarHorarioAerolinea(Id) {
    $("#divHorarioAerolinea").empty();
    $("#divHorarioAerolinea").load("/Administracion/HorariosAerolineas/Eliminar/" + Id);
}

function EditarHorarioAerolinea(Id) {
    $("#divHorarioAerolinea").empty();
    $("#divHorarioAerolinea").load("/Administracion/HorariosAerolineas/Editar/" + Id);
}

function EliminarHorarioAerolineaPost() {
    $.ajax({
        type: 'POST',
        url: "/Administracion/HorariosAerolineas/Eliminar",
        data:
        {
            horarioAerolineaOtd: {
                Id: $("#IdEliminarHorarioAerolinea").val()
            }
        },
        success: function (data) {
            $("#divMensajeHorarioAerolinea").empty();
            $("#divHorarioAerolinea").empty();
            if (data) {
                $("#BtnAbrirModalElim").click();
                //document.getElementById('divMensajeHorarioAerolinea').innerHTML = '<div class="alert alert-warning fade show" role="alert"><p> Horario Eliminado exitosamente </p></div>';
                $("#divHorarioAerolinea").load("/Administracion/HorariosAerolineas/Principal");
            }
            else {
                document.getElementById('divMensajeHorarioAerolinea').innerHTML = '<div class="alert alert-dark fade show" role="alert"><p> No se pudo eliminar Horario de aerolínea. </p></div>';
                $("#divHorarioAerolinea").load("/Administracion/HorariosAerolineas/Principal");
            }
        },
        error: function (data) {
            $("#mensajeGuardado").empty();
            document.getElementById('divMensajeHorarioAerolinea').innerHTML = '<div class="alert alert-dark fade show" role="alert"><p> No se pudo eliminar Horario de aerolínea. </p></div>';
            $("#divHorarioAerolinea").load("/Administracion/HorariosAerolineas/Principal");
        }
    })

}

 

function ListarAerolineas() {
    $("#divAerolineas").empty();
    $("#divAerolineas").load("/Administracion/Aerolinea/Principal");
}

function InsertarUsuario() {
    $("#divCrearUsuario").empty();
    $("#divCrearUsuario").load("/Administracion/AdmonGeneral/Insertar");
}

function InsertarUsuarioPost() {
    $.ajax({
        type: 'POST',
        url: "/Administracion/AdmonGeneral/Insertar",
        data:
        {
            usuarioOtd: {
                UserName: $("#UserName").val(),
                Email: $("#Email").val(),
                PhoneNumber: $("#PhoneNumber").val(),
                Nombre: $("#Nombre").val(),
                Apellido: $("#Apellido").val(),
                Cargo: $("#Cargo").val(),
                Telefono: $("#Telefono").val(),
                TipoDocumento: $("#TipoDocumento").val(),
                NumeroDocumento: $("#NumeroDocumento").val()
            }
        }
    });
}

function EliminarUsuarioGet(Id) {
    $("#divEliminarUsuario").empty();
    $("#divEliminarUsuario").load("/Administracion/AdmonGeneral/EliminarUser/" + Id);
}

function EditarUsuario(Id) {
    $("#divEditarUsuario").empty();
    $("#divEditarUsuario").load("/Administracion/AdmonGeneral/Actualizar/" + Id);
}
function EliminarUsuario(Id, nombre) {
    $("#divEliminarUsuario").text(nombre);
    $("#divEliminarUsuarioid").text(Id);
  }
function EliminarUserPost() {
    var dato = $("#divEliminarUsuarioid").text();
     $.ajax({
        type: 'POST',
        url: "/Administracion/AdmonGeneral/EliminarUsuario",
        data:
        {
            Id: dato
        }
    });
}
function ValidaLongitud( ) {
    var dato = $("#fechaInicio").val();    
    if (dato.length > 10) {        
        $("#fechaInicio").val("0001-01-01");
    }
    
}

function EditarUsuarioPost() {
    $.ajax({
        type: 'POST',
        url: "/Administracion/AdmonGeneral/Actualizar",
        data:
        {
            usuarioOtd: {
                Id: $("#Id").val(),
                UserName: $("#UserName").val(),
                Email: $("#Email").val(),
                PhoneNumber: $("#PhoneNumber").val(),
                Nombre: $("#Nombre").val(),
                Apellido: $("#Apellido").val(),
                Cargo: $("#Cargo").val(),
                Telefono: $("#Telefono").val(),
                TipoDocumento: $("#TipoDocumento").val(),
                NumeroDocumento: $("#NumeroDocumento").val()
            }
        }
    });

}

function DetalleTicket(Id) {
    //console.log("DetalleTicket");
    $("#divDetalleTicket").empty();
    $("#divDetalleTicket").load("/Contactenos/Tickets/Detalle/" + Id);
}

function ConsultarVuelos(fecha, id) {
    //console.log("ConsultarVuelos");
    $("#divCargarVuelos").empty();
    $("#divCargarVuelos").load("/Consulta/Consulta/ConsultarVuelos?fecha=" + fecha+"&id="+id);
}

function ConsultarNovedades(id) {
    //console.log("ConsultarNovedades");
    $("#divCargarNovedades").empty();
    $("#divCargarNovedades").load("/Consulta/Consulta/ConsultarNovedades?idOperacion=" + id);
    $("#OcultarFormulario").hide();
}


function ConsultarNovedadesNotificacionesPendiente() {
    $("#OcultarFormulario").show();
    $("#divCargarNovedades").empty();
    //window.location.href = '/Administracion/AdmonGeneral/Principal';
}






function GoValidaManual(id) {
    window.location.href = '/CargaInformacion/Vuelos/ValidacionManual/' + id;
}

function DescargarPDF(id) {
    window.location.href = '/Consulta/Consulta/DescargarPDF';
}

function DescargarExcel(id) {
    window.location.href = '/Consulta/Consulta/DescargarExcel';
}



$("#chkVNH").change(function () {
    /*
    if ($('#chkVNH').is(':checked') && $('#chkVIH').is(':checked')) {

        tblVuelosHistoricos.columns(0).search('').draw();
    }
    else if ($('#chkVNH').is(':checked') && $('#chkVIH').is(':not(:checked)')) {

        tblVuelosHistoricos.columns(0).search('DOM').draw();
    }
    else if ($('#chkVNH').is(':not(:checked)') && $('#chkVIH').is(':checked')) {

        tblVuelosHistoricos.columns(0).search('INT').draw();
    }
    else {
        tblVuelosHistoricos.columns(0).search('*').draw();
    }
    */
});

$("#chkVIH").change(function () {
    /*
    if ($('#chkVNH').is(':checked') && $('#chkVIH').is(':checked')) {

        tblVuelosHistoricos.columns(0).search('').draw();
    }
    else if ($('#chkVNH').is(':checked') && $('#chkVIH').is(':not(:checked)')) {

        tblVuelosHistoricos.columns(0).search('DOM').draw();
    }
    else if ($('#chkVNH').is(':not(:checked)') && $('#chkVIH').is(':checked')) {

        tblVuelosHistoricos.columns(0).search('INT').draw();
    }
    else {
        tblVuelosHistoricos.columns(0).search('*').draw();
    }
    */
});

$("#btnRangoFechas").click(function () {



    var tipoVueloHistorico = "";
    if ($('#chkVNH').is(':checked') && $('#chkVIH').is(':checked')) {
        tipoVueloHistorico = "";
    }
    else if ($('#chkVNH').is(':checked') && $('#chkVIH').is(':not(:checked)')) {
        tipoVueloHistorico = "DOM";
    }
    else if ($('#chkVNH').is(':not(:checked)') && $('#chkVIH').is(':checked')) {
        tipoVueloHistorico = "INT";
    }
    else {
        tipoVueloHistorico = "NONE";
    }


    $("#selectAerolineas").remove();

    tblVuelosHistoricos.destroy();
    tblVuelosHistoricos = $('#tblVuelosHistoricos').DataTable({
        "ajax": {
            "url": "/CargaInformacion/Vuelos/VuelosHistoricos?fechaInicio=" + $("#fechaInicio").val() + "&fechaFinal=" + $("#fechaFinal").val() +"&tipoVuelo="+tipoVueloHistorico,
            "dataSrc": ""
        },
        "columns": [
            { "data": "tipo", "visible": false },
            { "data": "destino", "visible": false },
            { "data": "nombreAerolinea" },
            { "data": "matricula" },
            { "data": "vuelo" },
            { "data": "fecha" },
            { "data": "hora" },
            {
                "data": null, "render": function (data, type, row) {
                    if (data.confirmacionPasajeros == 1) {
                        if (data.pdfPasajeros == 1) {
                            return '<button type="button" class="btn btn--simple" data-toggle="modal" data-target="#accionesVuelo" onclick="VerArchivo(\'PASAJEROS\',\'' + data.archivoPasajeros+'\');"><img src="/images/buscar.png" alt="Mostrar detalles"></button>';
                        }
                        else if (data.pdfPasajeros == 2) {
                            return '<button type="button" class="btn btn--simple" data-toggle="modal" data-target="#accionesVuelo" onclick="VerArchivo2(\'PASAJEROS\',\'' + data.archivoPasajeros+ '\','+data.id+');"><img src="/images/buscar.png" alt="Mostrar detalles"></button>';
                        } else {
                            return '<button type="button" class="btn btn--simple" data-toggle="modal" data-target="#accionesVuelo" onclick="VerListado(' + data.id + ',\'PASAJEROS\' , 1);"><img src="/images/buscar.png" alt="Mostrar detalles"></button>';
                        }

                    }
                    else {
                        return '<button type="button" class="btn btn--simple"><img src="/images/nulo.png" alt="Subir archivo" class="img-fluid" /> <span></span></button>';
                    }
                }
            },
            {
                "data": null, "render": function (data, type, row) {
                    if (data.confirmacionTransitos == 1) {
                        return '<button type="button" class="btn btn--simple" data-toggle="modal" data-target="#accionesVuelo" onclick="VerListado(' + data.id + ',\'TRANSITOS\' , 1);"><img src="/images/buscar.png" alt="Mostrar detalles"></button>';
                    }
                    else {
                        return '<button type="button" class="btn btn--simple"><img src="/images/nulo.png" alt="Subir archivo" class="img-fluid" /> <span></span></button>';
                    }
                }
            },
            {
                "data": null, "render": function (data, type, row) {
                    if (data.confirmacionManifiesto == 1) {
                        return '<button type="button" class="btn btn--simple" data-toggle="modal" data-target="#accionesVuelo" onclick="VerArchivo(\'MANIFIESTO\' , \'' + data.archivoManifiesto + '\');"><img src="/images/buscar.png" alt="Mostrar detalles"></button>';
                    }
                    else {
                        return '<button type="button" class="btn btn--simple"><img src="/images/nulo.png" alt="Subir archivo" class="img-fluid" /> <span></span></button>';
                    }
                }
            },
            {
                "data": null, "render": function (data, type, row) {
                    if (data.confirmacionGenDec == 1) {
                        return '<button type="button" class="btn btn--simple" data-toggle="modal" data-target="#accionesVuelo" onclick="VerArchivo(\'GENDEC\' , \'' + data.archivoGendec + '\');"><img src="/images/buscar.png" alt="Mostrar detalles"></button>';
                    }
                    else {
                        return '<button type="button" class="btn btn--simple"><img src="/images/nulo.png" alt="Subir archivo" class="img-fluid" /> <span></span></button>';
                    }
                }
            },
            {
                "data": null, "render": function (data, type, row) {                    
                    if (data.estadoProceso == '4') {
                        var btnValidaManual = '<button type="button" class="btn btn--simple" onclick="GoValidaManual(\'' + data.id + '\');"><img src="/images/Editar.png" alt="Mostrar detalles"></button>';
                    } else {
                        var btnValidaManual = '';
                    }
                    return btnValidaManual;
                }

            }
        ],
        "columnDefs": [{
            targets: 5, render: function (data) {
                //debugger;
                return moment(data).format('DD/MM/YYYY');
            }
        }],
        initComplete: function () {
            this.api().columns(2).every(function () {
                var column = this;
                var select = $('<select  class="d-block" id="selectAerolineas"><option value=""></option></select>')
                    .appendTo($(column.header()))
                    .on('change', function () {
                        var val = $.fn.dataTable.util.escapeRegex(
                            $(this).val()
                        );

                        column
                            .search(val ? '^' + val + '$' : '', true, false)
                            .draw();
                    });

                column.data().unique().sort().each(function (d, j) {
                    select.append('<option value="' + d + '">' + d + '</option>')
                });
            });
        }
    });
});

function MostrarObs(id) {

    $.ajax({
        type: 'GET',
        url: "/TransitoConexion/TransitoConexion/ObtenerObservacionRechazo?id=" + id ,
        success: function (data) {
        
            if (data) {                
                $("#lblmensa").text(data);
                $("#btnconfirmaFirmamensaje").click();                
            }

           
            else {
                $("#btnconfirmaFirmamensaje").click();     
            }
        },
        error: function (data) {
            //$("#mensajeGuardado").empty();
            console.log('Error al guardar los datos.');
        }
    })

  
}
//elopez INICIO bug 751 - ajuste componenete valor año formato
function ValidaLongitud() {
    var dato = $("#fechaInicio").val();    
    if (dato.length > 10) {        
        $("#fechaInicio").val("0001-01-01");
    }

} 

function ValidaLongitudF() {
    var dato = $("#fechaFinal").val();    
    if (dato.length > 10) {        
        $("#fechaFinal").val("0001-01-01");
    }

} 
 
//elopez FIN bug 751 - ajuste componenete valor año formato
function ValidaRechazados() {
    
    var sihay = 0;
    $('table tr').each(function () {

        if ($(this).find("td").eq(11).find(':checkbox').is(':checked')) {
            sihay = 1;
        }
    });
    if (sihay == 1) {
        $("#btnconfirmaFirma2").click();

    } else {
        $("#btnconfirmaFirma").click();
    }
}

$("#btnGuardarFirmar").on("click", function () {
    validaChecks();

   const radioButtons = document.querySelectorAll('input[name="pdfyexcel"]');

        for (const radioButton of radioButtons) {
            if (radioButton.checked) {
                nombreradioboton = radioButton.value;
                break;
            }
        }

    output.innerText = nombreradioboton ? `Usted Generará reporte  Con: ${nombreradioboton}` : `Usted no ha selecionado Nada`;
    
});

function validaChecks(bandera = "")
{

    if (bandera == "")
    {
        var cont = 0;
        $("input:checkbox:checked").each(function () {
            var check = $(this).attr("id");

            if (check == "chkAprobarTodos" || check == "chkRechazarTodos") {
                cont = cont - 1;
            }
            cont = cont + 1;
        });
        if (cont > 0) {
            $("#divMsg").hide();
            ValidaRechazados();
        } else {
            $("#divMsg").empty();
            document.getElementById('divMsg').innerHTML = '<div class="alert alert-warning fade show" role="alert">Favor seleccione algun elemento para procesar. </strong></div>';
            $("#divMsg").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    } else {
        var cont = 0;
        $("input:checkbox:checked").each(function () {
            var check = $(this).attr("id");

            if (check == "chkAprobarTodos" || check == "chkRechazarTodos") {
                cont = cont - 1;
            }
            cont = cont + 1;
        });
        if (cont > 0) {
            //console.log("correcto");
        } else {
            $('#chkRechazarTodos').prop('checked', false);
            $('#chkAprobarTodos').prop('checked', false);
        }
    }

}

//INICIO  Elopez 11-12-20109 Bug 758 - ajuste mutualidad cheks tránsitos y conexiones

$(".chekInt").change(function () {

    var check = $(this).attr("id");

    var tipoCheck = check.substr(0, 10);    

    if (tipoCheck == 'chkAprobar') {
        var checkCambio = '#chkRechazar-' + check.substr(11, 15) + '';        
        $(checkCambio).prop('checked', false);
        $('#chkRechazarTodos').prop('checked', false);
        
    }

    if (tipoCheck == 'chkRechaza') {
        var checkCambio = '#chkAprobar-' + check.substr(12, 15) + '';        
        $(checkCambio).prop('checked', false);
        $('#chkAprobarTodos').prop('checked', false);
        
    }

    validaChecks("true");
    //checkbox.prop('checked', false);
    //checkbox.change();
});

$("#chkAprobarTodos").change(function () {

    $('table tr').each(function () {
        if ($('#chkAprobarTodos').is(':checked')) {
            $(this).find("td").eq(10).find(':checkbox').prop('checked', true);
            //$('#chkRechazarTodos').is('display', 'none');
            $('#chkRechazarTodos').prop('checked', false);
            $('#chkRechazarTodos').change();

            //var checkboxes = $(this).closest('table').find(':checkbox');
            //checkboxes.prop('checked', $(this).is(':checked'));
        }
        else if ($('#chkAprobarTodos').is(':not(:checked)')) {
            $(this).find("td").eq(10).find(':checkbox').prop('checked', false);
        }

        //var pk = $(this).find("td").eq(10).find(':checkbox').attr('id');
        //var pk2 = $(this).find("td").eq(11).find(':checkbox').attr('id');

    });
});

$("#chkRechazarTodos").change(function () {

    $('table tr').each(function () {

        if ($('#chkRechazarTodos').is(':checked')) {
            $(this).find("td").eq(11).find(':checkbox').prop('checked', true);
            $('#chkAprobarTodos').prop('checked', false);
            $('#chkAprobarTodos').change();
            //var checkboxes = $(this).closest('table').find(':checkbox');
            //checkboxes.prop('checked', $(this).is(':checked'));
        }
        else if ($('#chkRechazarTodos').is(':not(:checked)')) {
            $(this).find("td").eq(11).find(':checkbox').prop('checked', false);
        }

        //var pk = $(this).find("td").eq(10).find(':checkbox').attr('id');
        //var pk2 = $(this).find("td").eq(11).find(':checkbox').attr('id');        
    });

});
//FIN  Elopez 11-12-20109 Bug 758 - ajuste mutualidad cheks tránsitos y conexiones

$("#btnFirmar").click(function () {
    var bandera = "0";
    $('.fchLlegada').each(function () {        
        if ($(this).val() == "") {
            bandera = "1";
        }
    });

    $('.horLlegada').each(function () {        
        if ($(this).val() == "") {
            bandera = "1";
        }
    });

    if (bandera == "0") {
        $('#formularioFirmar').submit();
        // Firmar transitos

    } else {
        alert("Es necesario ingresar una fecha de llegada y hora de llegada, Por favor revise los campos indicados.");
    }
    
});
$("#btnFirmar22").click(function () {
    if ($('#txtObservacion').val() == "") {
        alert('Debe colocar una observación');
         
    }
    else {

        var bandera = "0";
        $('.fchLlegada').each(function () {
            if ($(this).val() == "") {
                bandera = "1";
            }
        });

        $('.horLlegada').each(function () {
            if ($(this).val() == "") {
                bandera = "1";
            }
        });

        if (bandera == "0") {
            $('table tr').each(function () {
                var id;
                if ($(this).find("td").eq(11).find(':checkbox').is(':checked')) {
                    id = $(this).find("td").eq(12).html();
                    $.ajax({
                        type: 'GET',
                        url: "/TransitoConexion/TransitoConexion/Firmar22?id=" + id.trim() + "&Observacion=" + $('#txtObservacion').val(),
                        success: function (data) {

                            if (data) {


                            }
                            else {

                            }
                        },
                        error: function (data) {
                            //$("#mensajeGuardado").empty();
                            console.log('Error al guardar los datos.');
                        }, async: false
                    })
                }
            });

            $('#formularioFirmar').submit();
        } else {
            alert("Es necesario ingresar una fecha de llegada y hora de llegada, Por favor revise los campos indicados.");
        }

       
    }
    
});

$("#btnGuardarOperacion").click(function () {
    Loaderp('',1);
    $('button').prop("disabled",true);
    $('#formularioVuelos').submit();
});

$("#formularioVuelos").on("submit", function (eventObj) {
    var url = new URL(window.location.href);
    var tipoVuelo = url.searchParams.get("tipoVuelo") != null ? url.searchParams.get("tipoVuelo") : "";
    var tipoVueloHistorico = url.searchParams.get("tipoVueloHistorico") != null ? url.searchParams.get("tipoVueloHistorico") : "";
    var numeroVuelo = url.searchParams.get("numeroVuelo") != null ? url.searchParams.get("numeroVuelo") : "";
    var fechaInicio = url.searchParams.get("fechaInicio") != null ? url.searchParams.get("fechaInicio") : "";
    var fechaFinal = url.searchParams.get("fechaFinal") != null ? url.searchParams.get("fechaFinal") : "";
    fechaInicio = fechaInicio == '' ? $("#fechaInicio").val() : fechaInicio;
    fechaFinal = fechaFinal == '' ? $("#fechaFinal").val() : fechaFinal;
    var fechaInicioHistorico = url.searchParams.get("fechaInicioHistorico") != null ? url.searchParams.get("fechaInicioHistorico") : "";
    var fechaFinalHistorico = url.searchParams.get("fechaFinalHistorico") != null ? url.searchParams.get("fechaFinalHistorico") : "";
    $(this).append('<input type="hidden" id="tipoVuelo" name="tipoVuelo" value="' + tipoVuelo + '" />');
    $(this).append('<input type="hidden" id="tipoVueloHistorico" name="tipoVueloHistorico" value="' + tipoVueloHistorico + '" />');
    $(this).append('<input type="hidden" id="numeroVuelo" name="numeroVuelo" value="' + numeroVuelo + '" />');
    $(this).append('<input type="hidden" id="fechaInicio" name="fechaInicio" value="' + fechaInicio + '" />');
    $(this).append('<input type="hidden" id="fechaFinal" name="fechaFinal" value="' + fechaFinal + '" />');
    $(this).append('<input type="hidden" id="fechaInicioHistorico" name="fechaInicioHistorico" value="' + fechaInicioHistorico + '" />');
    $(this).append('<input type="hidden" id="fechaFinalHistorico" name="fechaFinalHistorico" value="' + fechaFinalHistorico + '" />');

    return true;
});

function CambioPrueba() {
    alert('hola');
}

function check(e) {
    tecla = (document.all) ? e.keyCode : e.which;

    patron = /[A-Za-z0-9]/;
    tecla_final = String.fromCharCode(tecla);

    return patron.test(tecla_final);
}

function PonerFecha() {
    var now = new Date();
    var day = ("0" + now.getDate()).slice(-2);
    var month = ("0" + (now.getMonth() + 1)).slice(-2);
    var today = now.getFullYear() + "-" + (month) + "-" + (day);
    $('#FechaVuelo').val(today);
}

PonerFecha();

$('#VuelosMsj').change(function () {
    if ($('#VuelosMsj').is(':checked')) {
        $('#TipoConsulta').val($('#VuelosMsj').val());
        $('#Asunto').attr("placeholder", "Número de vuelo");
        $("label[for='Asunto']").text("Número de vuelo");
        $('#FechaVuelo').show();
        $("label[for='FechaVuelo']").show();
        $('#Asunto').parent().removeClass("col-sm-6");
        $('#Asunto').parent().removeClass("col-sm-12");
        $('#Asunto').parent().addClass("col-sm-6");
        PonerFecha();
    }
});

$('#CobrosMsj').change(function () {
    if ($('#CobrosMsj').is(':checked')) {
        $('#TipoConsulta').val($('#CobrosMsj').val());
        $('#Asunto').attr("placeholder", "Número de vuelo");
        $("label[for='Asunto']").text("Número de vuelo");
        $('#FechaVuelo').show();
        $("label[for='FechaVuelo']").show();
        $('#Asunto').parent().removeClass("col-sm-6");
        $('#Asunto').parent().removeClass("col-sm-12");
        $('#Asunto').parent().addClass("col-sm-6");
        PonerFecha();
    }
});

$('#AnexosMsj').change(function () {
    if ($('#AnexosMsj').is(':checked')) {
        $('#TipoConsulta').val($('#AnexosMsj').val());
        $('#Asunto').attr("placeholder", "Número de factura");
        $("label[for='Asunto']").text("Número de factura");
        $('#FechaVuelo').hide();
        $("label[for='FechaVuelo']").hide();
        $('#Asunto').parent().removeClass("col-sm-6");
        $('#Asunto').parent().removeClass("col-sm-12");
        $('#Asunto').parent().addClass("col-sm-12");
    }
});

$('#FacturaMsj').change(function () {
    if ($('#FacturaMsj').is(':checked')) {
        $('#TipoConsulta').val($('#FacturaMsj').val());
        $('#Asunto').attr("placeholder", "Número de factura");
        $("label[for='Asunto']").text("Número de factura");
        $('#FechaVuelo').hide();
        $("label[for='FechaVuelo']").hide();
        $('#Asunto').parent().removeClass("col-sm-6");
        $('#Asunto').parent().removeClass("col-sm-12");
        $('#Asunto').parent().addClass("col-sm-12");
    }
});

$('#PQRMsj').change(function () {
    if ($('#PQRMsj').is(':checked')) {
        $('#TipoConsulta').val($('#PQRMsj').val());
        $('#Asunto').attr("placeholder", "Asunto");
        $("label[for='Asunto']").text("Asunto");
        $('#FechaVuelo').hide();
        $("label[for='FechaVuelo']").hide();
        $('#Asunto').parent().removeClass("col-sm-6");
        $('#Asunto').parent().removeClass("col-sm-12");
        $('#Asunto').parent().addClass("col-sm-12");
    }
});




$("#btnCargarLlegada").hide();
$("#btnCargarSalida").hide()

$(".btn__back").on("click", function () {
    $("body").addClass("modal-open2");
});

$(".btn__close").on("click", function () {
    $("body").removeClass("modal-open2");
});

CloseSuspend();


function VolverManual(fechaInicial, fechaFinal) {
    window.location.href = '/CargaInformacion/Vuelos/Principal?fechaInicio';
}