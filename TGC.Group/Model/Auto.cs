using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.GrupoMs.Camara;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Model;

namespace TGC.GroupoMs.Model
{
    public class Auto : MovingObject
    {
        

        
        public bool EsAutoJugador { get; set; }
        public float DireccionRuedas { get; set; } //negativo para la derecha, positivo para la izquierda

        
        public List<Arma> Armas { get; set; }
        public Arma ArmaSeleccionada { get; set; }
        
        
        public TgcMesh RuedaMainMesh { get; set; }
        //public int MyProperty { get; set; }

        public Ruedas RuedasTraseras { get; set; }
        public Ruedas RuedasDelanteras { get; set; }

        public Auto(string nombre, float vida, float avanceMaximo, float reversaMax,
                    float aceleracion,float desaceleracion,List<Arma> armas,
                    TgcMesh mesh,GameModel model,Ruedas ruedasAdelante,Ruedas ruedasAtras,TgcMesh ruedaMainMesh)
        {
            AvanceMax = avanceMaximo;
            ReversaMax = reversaMax;
            Desaceleracion = desaceleracion;
            InerciaNegativa = 1f;
            DireccionRuedas = 0f;
            Armas = armas;
            aceleracionVertical = 0f;

            Mesh = mesh;
            Aceleracion = aceleracion;

            GameModel = model;

            RuedasTraseras = ruedasAtras;
            RuedasDelanteras = ruedasAdelante;
            RuedaMainMesh = ruedaMainMesh; 



        }
        /// <summary>
        /// hace que la camara siga a este auto.
        /// </summary>
        /// <returns></returns>
        

        public void recibirDanio(int cant)
        {
            if (this.VidaMaxima < 0) {
                return;
            }
            this.Vida = -cant;
            if (this.Vida <= 0)
            {
                //TODO this.morir; //explota?
            }
            if (this.EsAutoJugador)
            {
                /*TODO mostrar en pantalla danio recibido,con 200ms seria suficiente, 
                podria ir sumandose para saber cuanto le sacamos en esa "rafaga" de disparos.*/
            }
            else
            {
                //TODO mostrar en pantalla danio dado
            }
        }

        public void Chocar(Vector3 angulo)
        {
            Velocidad = -Velocidad * 0.2f;
            PosicionRollback();
            
            //this.direccionFrente = + pi/2??

        }

        /// <summary>
        /// Llamado una vez por frame
        /// </summary>
        /// <param name="input"></param>
        public void Update(TgcD3dInput input)
        {

            if(!input.keyDown(Key.W) && !input.keyDown(Key.S))
                ProcesarInercia();
            if (!input.keyDown(Key.A) && !input.keyDown(Key.D))
                EnderezarRuedas();
            
            
            //1 acelerar
            if (input.keyDown(Key.W))
            {
                Acelerar();
            }

            //2 frenar, reversa
            if (input.keyDown(Key.S))
            {
                Frenar();
            }

            //3 izquierda TODO//girar ruedas
            if (input.keyDown(Key.A))
            {
                Doblar(0);
            }

            //4 derecha TODO //girar ruedas
            if (input.keyDown(Key.D))
            {
                Doblar(1);
            }

            //5 disparar TODO -- 
            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                //disparar 
                //this.ArmaSeleccionada.ammo--;
                //TODO falta evento de fin de ammo.
            }

            //6 cambiar de arma TODO
            if (input.buttonUp(TgcD3dInput.MouseButtons.BUTTON_RIGHT))
            { 

            }
            
            //7 freno de mano? maybe TODO
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.Space))
            {

            }
            if (input.keyDown(Key.Space))
            {
                Saltar();
            }
            else
            {
                //aceleracionVertical = -1; descomentar cuando el auto choque con el suelo.
            }

            RuedasDelanteras.Update(Mesh.Position,Velocidad,DireccionRuedas);
            RuedasTraseras.Update(Mesh.Position, Velocidad, DireccionRuedas);
            MoverMesh(); 
        }

        

        private void Doblar(int lado)
        {
            if(lado == 0) //doblo iz
            {
                DireccionRuedas -= 3f * GameModel.ElapsedTime;
            }
            if(lado == 1) //doblo der
            {
                DireccionRuedas += 3f * GameModel.ElapsedTime;
            }
        }

        private void EnderezarRuedas()
        {
            if (DireccionRuedas < 0.01 || DireccionRuedas > 0.01)
                DireccionRuedas = 0f;
            if (DireccionRuedas > 0)
                DireccionRuedas -= 6f * GameModel.ElapsedTime;
            if(DireccionRuedas<0)
                DireccionRuedas += 6f * GameModel.ElapsedTime;

        }

        /// <summary>
        /// Indica al mesh su nueva posicion y/o rotacion -- TODO
        /// </summary>
        private void MoverMesh()
        {
            PosicionAnterior = Mesh.Position;
            RotacionAnterior = Mesh.Rotation;
            //1- rotacion
            

            //2- traslacion
            Vector3 rotation = Mesh.Rotation; //obtengo la orientacion(rotacion) actual del mesh
            if (rotation == Vector3.Empty) //si la roteacion es nula, seteo una arbitraria
                rotation = new Vector3(0, 0, -1);
            //ahora multiplico la matriz rotacion por la velocidad del auto (Velocidad es una variable de la clase auto)
            rotation.Multiply(Velocidad); // esto guarda el resultado de la cuenta en la variable rotation

            //ahora q tengo el vector "movimiento", es decir, direccion y largo del movimiento deseado, me muevo.





            //3-saltar TODO
            
            rotation.Y += aceleracionVertical;

            

            Mesh.move(rotation);
            //4- las ruedas
            RuedasDelanteras.Update(Mesh.Position, Velocidad, DireccionRuedas);
            RuedasTraseras.Update(Mesh.Position, Velocidad, DireccionRuedas);

            //camera update
            if (CamaraAuto != null) //actualizo la posicion de la camara respecto de la del mesh
                CamaraAuto.Target = Mesh.Position;
        }

        /// <summary>
        /// lleva la velocidad del vehiculo lentamente a cero.
        /// </summary>
        private void ProcesarInercia()
        {
            
            if (Velocidad < 0.01f || Velocidad<0.01f)
            {
                Velocidad = 0f;
                return;
            }

            if (Velocidad > 0)
            {
                Velocidad -=InerciaNegativa * GameModel.ElapsedTime;
                return;
            }
            if(Velocidad < 0)
            {
                Velocidad +=InerciaNegativa * GameModel.ElapsedTime;
                return;
            }
           
            //efecto gravedad? -> TODO

        }

        /// <summary>
        /// rendereo.
        /// </summary>
        public void Render()
        {
            Mesh.render();
            RuedasDelanteras.Render();
            RuedasTraseras.Render();
            RuedaMainMesh.render();
        }
    }
}
