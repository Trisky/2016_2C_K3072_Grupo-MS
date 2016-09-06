using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.GroupoMs.Model;

namespace TGC.GroupoMs.utils
{
     class ConstructorInicial
    {
        //instancio posibles autos


        //instancio todas las armas

        List<Arma> Armas()
        {
            List<Arma> armasLst = new List<Arma>();
            armasLst.Add(new Model.Arma("ametralladora", 2, -1));
            armasLst.Add(new Arma("shotgun", 10, 50));
            //todo
            return armasLst;
        }
        List<Auto> Autos() {
            List<Auto> autosLst = new List<Auto>();
            autosLst.Add(new Auto("hummer", 100, 100, 50, 10, 7, new List<Arma>(), null));
            //todo
            return autosLst;
        }

    }
}
