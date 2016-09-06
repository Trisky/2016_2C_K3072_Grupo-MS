using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace TGC.GroupoMs.utils
{
    

    /// <summary>
    /// Tiene una lista con todos los meshes y sus nombres para facil acceso
    /// TODO = hacer que la clase sea un singleton
    /// </summary>
    public sealed class  MeshCollection
    {
        // singleton handling

        private MeshCollection()
        {
            this.MeshLst = new List<MeshElement>();
        }
        private static MeshCollection instance = null;
        public static MeshCollection GetInstance()
        {
            // create the instance only if the instance is null
            if (instance == null)
            {
                instance = new MeshCollection();
            }
            // Otherwise return the already existing instance
            return instance;
        }
        //
        private List<MeshElement> MeshLst { get; set; }

        

        public TgcMesh GetMeshPorNombre(string nombre)
        {
           var element  = this.MeshLst.First(e => e.Nombre == nombre);
           return element.Mesh;
        }

        /// <summary>
        /// agrega un mesh y su nombre a la coleccion de meshes
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="mesh"></param>
        /// <param name="tipoObjeto">recibe un objeto vacio que referencia a que tipo de objeto hace referencia el mesh.</param>
        public void NuevoMesh(string nombre, TgcMesh mesh, Object tipoObjeto)
        {
            MeshElement e = new MeshElement(nombre, mesh, tipoObjeto);
            this.MeshLst.Add(e);
        }
        

    }


    /// <summary>
    /// Un elemento de la meshCollection
    /// </summary>
    public class MeshElement
    {
        public MeshElement(string nombre, TgcMesh mesh, Object o)
        {
            this.Nombre = nombre;
            this.Mesh = mesh;
            this.ObjetoTipo = o;

        }
        public string Nombre { get; set; }
        public TgcMesh Mesh { get; set; }
        public Object ObjetoTipo { get; set; }
    }
}
