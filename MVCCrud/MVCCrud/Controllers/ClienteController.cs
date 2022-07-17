using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using MVCCrud.Models;
using MVCCrud.Models.ViewModels;

namespace MVCCrud.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente
        public ActionResult Index()
        {
            List<ListClienteViewModel> listaClientes;
            using (facturaEntities db = new facturaEntities())
            {
                listaClientes = ( from c in db.cliente
                                  select new ListClienteViewModel
                                  {
                                      id_cli = c.id_cli,
                                      nombre_cli = c.nombre_cli,
                                      cedula_cli = c.cedula_cli,
                                      correo_cli = c.correo_cli
                                  }).ToList();
            }
                return View(listaClientes);
        }

        public ActionResult Nuevo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(ClienteViewModel clienteModel)
        {
            try
            {
                //Validar el modelo
                if (ModelState.IsValid)
                {
                    HttpPostedFileBase pfb = Request.Files[0];
                    WebImage img = new WebImage(pfb.InputStream);
                    clienteModel.foto = img.GetBytes();
                    //Conexion a la base de datos y paso de datos del modelo a un
                    //objeto de tipo cliente
                    using(facturaEntities db = new facturaEntities())
                    {
                        var oCliente = new cliente();
                        oCliente.nombre_cli = clienteModel.nombre_cli;
                        oCliente.cedula_cli = clienteModel.cedula_cli;
                        oCliente.correo_cli = clienteModel.correo_cli;
                        oCliente.fechaNacimiento = clienteModel.fechaNacimiento;
                        oCliente.foto = clienteModel.foto;

                        //Almacenar en la base de datos el objeto cliente
                        db.cliente.Add(oCliente);
                        db.SaveChanges();
                    }
                    return Redirect("~/Cliente/");
                }
                return View(clienteModel);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Editar(int id) //parametro id
        {
            ClienteViewModel model = new ClienteViewModel();
            using (facturaEntities db = new facturaEntities())
            {
                var oCliente = db.cliente.Find(id);
                model.nombre_cli = oCliente.nombre_cli;
                model.cedula_cli = oCliente.cedula_cli;
                model.correo_cli = oCliente.correo_cli;
                model.fechaNacimiento = (DateTime)oCliente.fechaNacimiento;
                //Dato agregado recien 
                model.id_cli = oCliente.id_cli;
            }
            return View(model);
        }
        [HttpPost]
        public ActionResult Editar(ClienteViewModel clienteModel) //parametro id
        {
            try
            {
                //Validando el formulario
                if (ModelState.IsValid)
                {
                    using (facturaEntities db = new facturaEntities())
                    {
                        //Se busca el cliente para actualizar
                        var oCliente = db.cliente.Find(clienteModel.id_cli);
                        oCliente.nombre_cli = clienteModel.nombre_cli;
                        oCliente.cedula_cli = clienteModel.cedula_cli;
                        oCliente.correo_cli = clienteModel.correo_cli;
                        oCliente.fechaNacimiento = (DateTime)clienteModel.fechaNacimiento;

                        //Envair los datos para que sean actualizados
                        //Se mantiene el estado, se haga la actualizacion y que no se vuelva agregar
                        db.Entry(oCliente).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    return Redirect("~/Cliente/");
                }

                return View(clienteModel);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult Eliminar(int id)
        {
            using(facturaEntities db = new facturaEntities())
            {
                var oCliente = db.cliente.Find(id);
                db.cliente.Remove(oCliente);
                db.SaveChanges();
            }

            return Redirect("~/Cliente/");
        }
    }
}