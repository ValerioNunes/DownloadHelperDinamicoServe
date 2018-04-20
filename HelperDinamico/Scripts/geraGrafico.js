var urlbase = "/hd/EventoHelperDinamico/";

function main() {
    var util = new Util();
    var categorias = ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'];
    var grafico = util.Grafico('grafico', 'line', 'Eventos HELPER por mês', categorias, 'Quantidade de Eventos');

    util.Ajax(urlbase + 'GetEventosHelper', null).then(function (data) {
        if (data) {
            //console.log(data)
            var names = ["Falhas", "Defeitos"];
            var colors = ["red", "gray"];
            for (var i = 0; i < data.length; i++)
                grafico.addSeries({
                    name: names[i],
                    data: data[i],
                    color: colors[i]
                });
        }
    });

    var graficoporloco = util.Grafico('graficoporloco', 'column', 'Eventos por Locomotiva no Mês', categorias, 'Quantidade de Eventos');

    util.Ajax(urlbase + 'GetEventosLocomotiva', null).then(function (data) {
        if (data) {

            for (var k in data) {
                graficoporloco.addSeries({
                    name: data[k].Nome,
                    data: data[k].eventos
                })
            }


        }
    });
}

main();