# 🎟️ Sistema de Atención de Tickets en Tiempo Real

## 📌 Descripción

Este proyecto implementa un sistema de atención de tickets en tiempo real desarrollado en **C# (.NET)**, aplicando conceptos avanzados de programación como:

* Programación concurrente
* Sincronización de hilos
* Monitores
* Manejo de excepciones
* Programación orientada a eventos
* Persistencia con base de datos (Entity Framework Core + SQL Server)

El sistema simula la creación y atención de tickets por múltiples agentes trabajando de manera concurrente.

---

## 🎯 Objetivos del Proyecto

* Aplicar estructuras de concurrencia en un entorno realista
* Implementar sincronización segura de recursos compartidos
* Gestionar eventos en tiempo real
* Diseñar una base de datos relacional normalizada
* Desarrollar código limpio y modular siguiendo buenas prácticas

---

## ⚙️ Tecnologías utilizadas

* 💻 C# (.NET 8)
* 🧠 Entity Framework Core
* 🗄️ SQL Server
* 🔄 Multithreading (`Thread`, `Monitor`, `lock`)
* 🧩 Programación orientada a objetos

---

## 🧱 Arquitectura del Proyecto

```
ProyectoFinalTickets/
│
├── Models/          → Entidades del sistema (Ticket, Estado, Prioridad, etc.)
├── Data/            → DbContext y configuración de base de datos
├── Services/        → Lógica de negocio
├── Concurrency/     → Hilos, monitores y ejecución concurrente
├── Exceptions/      → Manejo de errores personalizados
└── Program.cs       → Punto de entrada y menú interactivo
```

---

## 🗄️ Modelo de Base de Datos

El sistema utiliza un modelo normalizado con relaciones:

### Tablas principales:

* `Tickets`
* `Estados`
* `Prioridades`
* `Eventos`
* `Agentes`

### Relaciones:

* Un **Ticket** tiene un **Estado**
* Un **Ticket** tiene una **Prioridad**
* Los eventos registran acciones del sistema

---

## 🚀 Funcionalidades

* ✅ Creación manual de tickets
* 🤖 Generación automática de tickets (background)
* 👨‍💻 Atención concurrente por múltiples agentes
* 🔒 Sincronización mediante monitores (`Monitor.Wait / Pulse`)
* 📊 Registro de eventos del sistema
* 🎨 Visualización con colores en consola:

  * Estado del ticket
  * Prioridad del ticket
* 📈 Consulta de tickets y eventos
* ⚠️ Detección de saturación del sistema

---

## 🧵 Concurrencia y sincronización

El sistema utiliza:

* **Hilos independientes**:

  * Generador de tickets
  * Agentes de atención
  * Supervisor del sistema

* **Monitor de tickets**:

  * Cola compartida sincronizada
  * Uso de:

    * `lock`
    * `Monitor.Wait`
    * `Monitor.PulseAll`

Esto evita condiciones de carrera y garantiza acceso seguro a los recursos.

---

## 🔔 Manejo de eventos

El sistema genera eventos como:

* Creación de tickets
* Asignación de agentes
* Cierre de tickets
* Alertas de saturación

Estos eventos se almacenan en base de datos y se pueden consultar desde el menú.

---

## ⚠️ Manejo de excepciones

Se implementan excepciones personalizadas:

* `TicketInvalidoException`
* `DatabaseException`

Además, se manejan errores en:

* Entrada de usuario
* Operaciones de base de datos
* Ejecución concurrente

---

## 🛠️ Instalación y ejecución

### 1. Clonar el repositorio

```bash
git clone https://github.com/tu-usuario/tu-repo.git](https://github.com/MikeCruz17/ProyectoSistemaAtencionTickets-IngSoft.git
cd ProyectoSistemaAtencionTickets
```

### 2. Configurar la cadena de conexión

En `Program.cs`:

```csharp
string connectionString =
    "Server=localhost;Database=ProyectoFinalTicketsDB;Trusted_Connection=True;TrustServerCertificate=True;";
```

> Ajusta según tu instancia de SQL Server.

---

### 3. Ejecutar con Visual Studio

---

## 📋 Uso del sistema

Menú principal:

```
1. Crear ticket manual
2. Listar tickets
3. Ver eventos
4. Ver cantidad de tickets pendientes
5. Salir
```

---

## 🎨 Visualización en consola

* **Estado**

  * ✓ Cerrado → Verde
  * ⏳ En Proceso → Amarillo
  * ● Pendiente → Cyan

* **Prioridad**

  * 🔴 Alta → Rojo
  * 🟡 Media → Amarillo
  * 🟢 Baja → Verde

---

## 📌 Consideraciones técnicas

* Cada hilo usa su propio `DbContext` (EF Core no es thread-safe)
* Se evita saturación de eventos mediante control de frecuencia
* Los logs en tiempo real no se muestran en consola para evitar conflictos con el menú
* Se mantiene persistencia completa en base de datos

---

## 📚 Temas aplicados del curso

Este proyecto cubre:

* Manejo de excepciones
* Programación concurrente
* Sincronización
* Monitores
* Programación de eventos en tiempo real
* Base de datos relacional

---

## 👨‍🎓 Autor

* Nombre: Miguel Angel Cruz
* Curso: 24-0195
* Profesor: Joerlyn Morfe

---

## 📄 Licencia

Este proyecto es de uso académico para la materia de Ingeniería de Software en Tiempo Real en UNIBE.
