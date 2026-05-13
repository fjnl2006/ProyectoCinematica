# Castle Defense - Física para 3D y Simulación

## Descripción del Proyecto
**Castle Defense** es un *Tower Defense* estratégico ambientado en una isla fortificada. El jugador asume el rol de comandante de la artillería pesada del castillo, con la misión crítica de proteger el corazón de la fortaleza frente a incesantes hordas de muertos vivientes.

El núcleo del juego reside en el control directo de **4 cañones** estratégicamente situados en los puentes de acceso. La supervivencia depende de la puntería, la gestión de tiempos de recarga y el uso inteligente de las trampas físicas del escenario.

## Mecánicas Principales

### 1. Sistema de Artillería
* **Control Manual:** Apuntado dinámico mediante el ratón.
* **Física de Proyectiles:** Los disparos cuentan con caída de bala (gravedad) y rastro visual mediante `LineRenderer`.
* **Daño de Área:** Impactos explosivos con daño AOE para controlar grandes grupos.

### 2. Compuertas y Puentes (Física Aplicada)
* **Puerta Principal:** Actuable mediante teclado. Utiliza un sistema motorizado con *cooldown*.
* **Letalidad Dinámica:** Gracias a un script de detección de velocidad angular, la puerta solo elimina enemigos cuando está en movimiento (subiendo o bajando), permitiendo el paso cuando está en reposo.
* **Física de Cadenas:** Implementación de cadenas realistas mediante una jerarquía de *Empties* y `HingeJoints`, optimizada para evitar errores de colisión cóncava.

### 3. Sistema de Enemigos
* **Oleadas Asíncronas:** Los enemigos aparecen de forma constante pero asimétrica desde 4 islas diferentes.
* **Spawning Aleatorio:** La cantidad y el ritmo de las hordas varían para evitar patrones previsibles, obligando al jugador a vigilar todos los frentes simultáneamente.

## Controles
| Acción | Tecla / Input |
| :--- | :--- |
| **Disparar** | Clic Izquierdo |
| **Apuntar** | Movimiento del Ratón |
| **Cambiar de Cañón/Puente** | Teclas 1, 2, 3, 4 |
| **Cámara Zenital** | Espacio |
| **Cambiar Vista de Cámara** | Tab |
| **Puerta Principal** | Q |
| **Puerta de Rejas** | E |

## Decisiones de Diseño y Soluciones Técnicas
Durante el desarrollo se enfrentaron varios retos de simulación física:
* **Problema de las Cadenas:** Inicialmente, el uso de `Mesh Colliders` complejos provocaba errores de "Concave Mesh". Se solucionó sustituyéndolos por una cadena de huesos físicos (*Rigidbodies* con *HingeJoints*) en objetos vacíos.
* **Optimización de Trampas:** Para evitar que la puerta matara enemigos simplemente por estar cerrada, se implementó una validación en el script `KillEnemy` que consulta el `angularVelocity` del Rigidbody antes de aplicar la lógica de muerte.

## Instalación
1. Clona el repositorio: `git clone https://github.com/tu-usuario/castle-defense.git`
2. Abre el proyecto en **Unity 2021.3** o superior.
3. Carga la escena `Demo.unity` ubicada en `Assets/Scenes/`.
