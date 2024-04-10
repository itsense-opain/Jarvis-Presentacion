$(".alert__bell").hide();

$(".btn-block").on("click", function () {		
	$(".detalleTicket").hide();
	$(".detalleTicket").css("display", "none");
	$(".detalleTicket").attr("aria-hidden", "true");
	$(".detalleTicket").attr("aria-modal", "");		
	$(".detalleTicket").removeClass("show");
});

$("#notify").empty();

$.ajax({
	type: 'GET',				
	url: "/Consulta/Consulta/NotifyNotRead",
	success: function (data) {			
		var cont = 0;	
		
		$.each(data, function () {
			cont = cont + 1;
			var href = '#';
			
			if (this.rol_responsable == 'ADMINISTRADOR') {
                href = '/Contactenos/Tickets/Principal?notifyADM=' + this.id_item + '&id=' + this.id;
                var addMnu = "<div class='dropdown-item'><strong> Nuevo Ticket Nro. " + this.id_item + "</strong><br>Asunto: " + this.descripcion.substring(0,30) + "..." + " <br> <a href='" + href + "'>Ver detalles</a></div>";
			}
			else if (this.rol_responsable == 'AEROLINEA') {
				href = '/Contactenos/Tickets/Principal?notifyADM=' + this.id_item + '&id=' + this.id;
                var addMnu = "<div class='dropdown-item'><strong> Rpta Ticket Nro. " + this.id_item + "</strong><br>Asunto: " + this.descripcion.substring(0, 30) + "..." + " <br> <a href='" + href + "'>Ver detalles</a></div>";					
			}
				
			$("#notify").append(addMnu)
			$("#notify").change();
		});
		if (cont > 0) {
			$(".alert__bell").show();
		}
	},
	error: function (data) {			
		console.log('Ha surgido un error al traer notificaciones.');			
	}
})

var getUrlParameter = function getUrlParameter(sParam) {
	var sPageURL = window.location.search.substring(1),
		sURLVariables = sPageURL.split('&'),
		sParameterName,
		i;

	for (i = 0; i < sURLVariables.length; i++) {
		sParameterName = sURLVariables[i].split('=');

		if (sParameterName[0] === sParam) {
			return sParameterName[1] === undefined ? true : decodeURIComponent(sParameterName[1]);
		}
	}
};


var notifyADM = getUrlParameter('notifyADM');
var id = getUrlParameter('id');

if (notifyADM != undefined) {
	updateStateNotify(id);		

	DetalleTicket(notifyADM);
}

var notifyAERO = getUrlParameter('notifyAERO');

if (notifyAERO != undefined) {
	updateStateNotify(id);

}

var SolNotifyTTC = getUrlParameter('SolNotifyTTC');

if (SolNotifyTTC != undefined) {
	updateStateNotify(id);
}

var ResNotifyTTC = getUrlParameter('ResNotifyTTC');

if (ResNotifyTTC != undefined) {
	updateStateNotify(id);
}

function DetalleTicket(Id) {
	$("#divDetalleTicket").empty();
	$("#divDetalleTicket").load("/Contactenos/Tickets/Detalle/" + Id);
	$(".detalleTicket").show();
	$(".detalleTicket").attr("aria-modal", "true");
	$(".detalleTicket").addClass("show");
}

function updateStateNotify(notify) {
	$.ajax({
		type: 'GET',
		url: "/Consulta/Consulta/NotifyUpd?notify=" + notify,
		success: function (data) {
			$("#notify").change();
		},
		error: function (data) {
			console.log('Error al consultar notificacion.');
		}
	})
}