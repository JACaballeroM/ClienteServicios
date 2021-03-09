using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LC.ServiceLC;
//using LC.ServiceLC1;
//using LC.ServiceLC2;

namespace LC
{
    public partial class Home : System.Web.UI.Page
    {

        private bool val1(Decimal x) {
           
        return x == ((100M - Math.Min(40M, 50 * 1)) / 100M);
        }

        private void leeLC(tr_lineaCapturaGeneral[] lineas)
        {

            if (lineas.Length > 0)
            {
                tr_lineaCapturaGeneral linea = lineas.FirstOrDefault();

                if (!string.IsNullOrEmpty(linea.lineaCaptura))
                {



                    var lineaCaptura = linea.lineaCaptura.Substring(0, 20);

                    var caja = linea.caja;
                    var error = linea.error;
                    var error_descripcion = linea.error_descripcion;
                    var fcobro = linea.fcobro;
                    var intImpuesto = linea.intImpuesto;
                    //var lineaCaptura = linea.lineaCaptura;
                    var partida = linea.partida;
                    var rafagaPago = linea.rafagaPago;
                    var referencia = linea.referencia;
                    //var lineaCaptura = linea.lineaCaptura;


                }

            }

        }

        private tp_lineaCapturaGeneral cargaParametros(tp_lineaCapturaGeneral pregunta)
        {

            pregunta.usuario = "kioskos";
            pregunta.password = "bf2579b6ccd26082aaacf2b8e0774ff5";
            pregunta.intImpuesto = 77;
            pregunta.referencia = "0701047";
            pregunta.fechaLimPP = String.Format("{0}-{1:00}-{2:00}", 2021, 1, 1);

            pregunta.concepto = "02";
            pregunta.usuarioGen = "gcalvillo";

            detalleCamposMatriz datos = new detalleCamposMatriz();
            datos.ctapredial = "027070190213";
            datos.ctarfc = "LASD550129S95";
            datos.recargo2 = "0";
            datos.vencim = "20210126";
            datos.numfolio = "0.00";
            datos.liquidacion = "0.00";
            datos.otros = "0.00";
            datos.bonifica = "0.00";
            datos.recargo1 = "0.00";
            datos.id_pago = "01020701";
            datos.ctaeconum = "86";
            pregunta.arrayDatos = new detalleCamposMatriz[1];
            pregunta.arrayDatos[0] = datos;

            datos.impuesto = "3900.16";
            datos.total = "3900.16";

            double tot = 3900.16;
            if ((tot - Math.Truncate(tot)) > 0.50)
                pregunta.totalV = (int)Math.Ceiling(tot);
            else
                pregunta.totalV = (int)Math.Floor(tot);

            return pregunta;
        }
        private tr_lineaCapturaGeneral[] cargaDatosLC1(ServicesPortTypeClient clienteLC,tp_lineaCapturaGeneral pregunta)
        {
            tr_lineaCapturaGeneral[] lineas = clienteLC.solicitaLineaCapturaGen(cargaParametros(pregunta));
            clienteLC.Close();
            return lineas;
        }

        /*private tr_lineaCapturaGeneral[] cargaDatosLC2(lineaCapturaGenWs_secureServerPortTypeClient clienteLC, tp_lineaCapturaGeneral pregunta)
        {
            tr_lineaCapturaGeneral[] lineas = clienteLC.solicitaLineaCapturaGen(cargaParametros(pregunta));
            clienteLC.Close();
            return lineas;
        }*/


        protected void AplicarAcceso(ServiceToken.WS_Recibe_AvaluoClient servicio)
        {
            var scope = new System.ServiceModel.OperationContextScope(servicio.InnerChannel);
            System.ServiceModel.OperationContext context = System.ServiceModel.OperationContext.Current;

            string user = "Usrpruebas";
            string passw = "qa7350r3rlac6mx-45";

            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels.MessageHeader.CreateHeader("usuario","IDEAS.Avametrica",Abase64(user));
            context.OutgoingMessageHeaders.Add(header);
            


            header = System.ServiceModel.Channels.MessageHeader.CreateHeader("contrasenia", "IDEAS.Avametrica", Abase64(passw));
            context.OutgoingMessageHeaders.Add(header);
        }

        protected string Abase64(string strOriginal)
        {
            byte[] byt = System.Text.Encoding.UTF8.GetBytes(strOriginal);
            return Convert.ToBase64String(byt);
        }

        private void solicitarToken()
        {
            ServiceToken.WS_Recibe_AvaluoClient clienteToken = new ServiceToken.WS_Recibe_AvaluoClient();
            AplicarAcceso(clienteToken);
            clienteToken.obtenertoken("F2154");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ServicesPortTypeClient clienteLC = new ServicesPortTypeClient();
            //lineaCapturaGenWs_secureServerPortTypeClient clienteLC = new lineaCapturaGenWs_secureServerPortTypeClient();
            tp_lineaCapturaGeneral pregunta = new tp_lineaCapturaGeneral();
            leeLC(cargaDatosLC1(clienteLC, pregunta));

            ServiceToken.WS_Recibe_AvaluoClient clienteToken = new ServiceToken.WS_Recibe_AvaluoClient();
            AplicarAcceso(clienteToken);

            solicitarToken();
            System.Threading.Thread.Sleep(5000);
            string token = "";
            var registroAvaluo = new ServiceToken.DatosRegistroAvaluo();
            registroAvaluo.AvaluoXML = "CadenaXML";
            registroAvaluo.Folio_Interno = "F2154";
            registroAvaluo.Folio_Usuario = "U4566";
            registroAvaluo.token = token;
            if (clienteToken.BandejaAvaluoXML(registroAvaluo) == true)
            {
                Console.WriteLine("Avaluo Entregado");
            }
            else
            {
                Console.WriteLine("Avaluo NO Entregado");
            }

        }
    }
}
