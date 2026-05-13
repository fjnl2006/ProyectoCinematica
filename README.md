# Castle Defense - Física para 3D y Simulación

## Descripción del Proyecto
**Castle Defense** es un *Tower Defense* estratégico ambientado en una isla fortificada. El jugador asume el rol de comandante de la artillería pesada del castillo, con la misión crítica de proteger el corazón de la fortaleza frente a incesantes hordas de muertos vivientes.

El núcleo del juego reside en el control directo de **4 cañones** estratégicamente situados en los puentes de acceso. La supervivencia depende de la puntería, la gestión de tiempos de recarga y el uso inteligente de las trampas físicas del escenario.

## Mecánicas Principales

### 1. Sistema de Artillería y Trayectorias
* **Control Manual:** Apuntado dinámico mediante el ratón.
* **Física de Proyectiles:** Los disparos cuentan con caída de bala (gravedad) y rastro visual mediante `LineRenderer`.
* **Visualización de Predicción:** Implementación de un sistema de trazado de trayectoria parabólica en tiempo real.

### 2. Compuertas y Puentes (Física Aplicada)
* **Puerta Principal:** Actuable mediante teclado con sistema motorizado y *cooldown*.
* **Letalidad Dinámica:** La puerta solo elimina enemigos cuando su `angularVelocity` supera un umbral, permitiendo que sea segura en reposo.
* **Física de Cadenas:** Simulación mediante jerarquía de *Empties* y `HingeJoints`, optimizada para rendimiento y estabilidad.

### 3. Sistema de Enemigos y Spawning
* **Oleadas Asíncronas:** Los enemigos aparecen desde 4 islas diferentes con ritmos descompasados.
* **Asimetría Táctica:** Flujo constante de enemigos que evita patrones simétricos, forzando la atención multifrente.

## Retos Técnicos y Soluciones Implementadas

### 💡 Desafío de Escalabilidad: Spawners Multifrente
Uno de los mayores retos fue el **escalado de los spawners** tras expandir el juego a 4 puentes. La gestión individual de cada flujo de enemigos presentaba complicaciones en la sincronización y el rendimiento.
* **Solución:** Se refactorizó el sistema de spawn para trabajar de forma asíncrona, permitiendo que cada puente gestione su propia carga sin interferir en el ritmo global, logrando una experiencia de "asedio total" fluida.

### 🎯 Precisión Balística: El Sistema de Trayectorias
La implementación de la **trayectoria visual del cañón** presentó complicaciones matemáticas significativas. Lograr que el `LineRenderer` coincidiera exactamente con la parábola física real de la bala de cañón requirió un ajuste fino de los vectores de fuerza y gravedad.
* **Solución:** Se desarrolló un algoritmo de predicción que calcula la posición del proyectil en múltiples puntos futuros basándose en la velocidad inicial y la gravedad de Unity, garantizando que el jugador siempre tenga una referencia visual fiable antes de disparar.

### 🔗 Estabilidad Física: El Problema de las Cadenas
El uso de modelos complejos para las cadenas generaba errores críticos de colisión ("Concave Mesh Colliders").
* **Solución:** Se migró a un sistema de colisionadores simplificados sobre objetos vacíos, eliminando los errores de consola y evitando las "explosiones" físicas al interactuar con la puerta.

## Controles
| Acción | Tecla / Input |
| :--- | :--- |
| **Disparar** | Clic Izquierdo |
| **Apuntar** | Movimiento del Ratón |
| **Cambiar de Puente** | Teclas 1, 2, 3, 4 |
| **Cámara Zenital** | Espacio |
| **Cambiar Cámara** | Tab |
| **Puerta Principal** | Q |
| **Puerta de Rejas** | E |

---
*Proyecto desarrollado para el Grado en Diseño y Desarrollo de Videojuegos.*
