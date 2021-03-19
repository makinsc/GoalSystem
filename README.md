# Bienvenido a GoalSystem aplicación Inventario!

Este proyecto es una gestión básica de una entidad de inventario a través de una **API REST**, donde se ha intentado plasmar una arquitectura orientada a dominio con **.Net Core 3.1** y **Entity Framework Core**.  
Como sistema de notificaciones en tiempo real con los clientes, hemos elegido **SignalR**, a través del cual, emitiremos una serie de mensajes cuando la fecha de expedición de un elemento haya pasado o cuando se saque un elemento del inventario. 
Para el aseguramiento de la calidad se han desarrollado pruebas unitarias y de integración utilizando **MSTest** y **XUnit**.


# Overview de la arquitectura

A continuación se muestra una imagen de los componentes de la arquitectura empreada en el proyecto.

![enter image description here](https://raw.githubusercontent.com/makinsc/GoalSystem/main/overview.png?token=AASZMVZYTBVTCEJA2WTZ7GTAKRS3Y)

Se pretende utilizar una arquitectura que cumpla con los principios de programación SOLID, que sea fácil de mantener, no excesivamente compleja de evolucionar para que sea escalable y que nos permita testear cada componente de manera individual e integrada para poder asegurar la calidad del código y de los entregables.
Es por ello que vamos a seguir el marco de Arquitectura N-Capas Orientada al Dominio como parte del diseño de arquitectura Domain Driven Design (DDD). El objetivo será tener el mínimo acoplamiento posible entre el Modelo de Dominio (lógica y reglas de Negocio) y el resto de las capas del sistema (Presentación, Persistencia, etc).

## Revisión de la arquitectura

### Capa de Servicios Distribuidos
En esta capa tendremos la aplicación de servicios Rest API, encargada de proporcionar la información requerida por los frontales. Además dispondrá de un Hub de notificaciones en tiempo real por WebSocket, diseñado con tecnología **SignalR**
En la el proyecto de **Api Rest** tendremos definida la inyección de dependencias, así como los servicios en background autohospedados de inspección de elementos caducados, que lanzará una notificación hacia los clientes conectados en el Hub de notificaciones.

### Capa de Aplicación
Esta capa define las tareas y casos de usuario que la aplicación debe realizar, realizando ahí la lógica específica de la aplicación si es que la hubiera; además enlaza con la capa de dominio, que son los que implementan el proceso y las reglas de negocio.

### Capa de Dominio
El corazón del software. Implementa y encapsula toda la lógica del negocio y aplica las reglas del dominio. Es totalmente independiente de la capa de persistencia, consiguiendo así que el software sea dirigido por el dominio y no por el dato, o el motor de base de datos.
>No se ha implementado un sistema de validaciones por no merecer la pena para el proyecto de ejemplo, pero se recomienda en un proyecto real, usar el patrón validator con Fluent Validator, con lo cual conseguiríamos una separación de responsabilidades, ganando en extensibilidad y mantenibilidad.

Desde la capa de dominio, se lanzarán notificaciones en tiempo real cuando se extraiga un elemento del inventario. Esto permitirá a los clientes estar informados en tiempo real de los elementos que son extraídos del inventario. 

### Capa de Infraestructura
Expone, de manera desacoplada, el acceso a datos a las capas superiores. Distinguimos las siguientes capas:
El Agente de Servicios Externos se comunicará con el bus de los servicios proporcionados por AGQ para implementar las
funcionalidades necesarias.
#### La Capa de Persistencia 
Persistirá los datos en un motor de base de datos **SQL Server alojado en Azure**, esta capa expondrá el acceso a dichos datos. Para el acceso a datos se utilizará **EntityFramework Core**, en su enfoque **Code First**, que a través del **Patrón repositorio genérico** y el patrón **UnitOfWork** conseguiremos una capa de acceso a datos muy flexible, versatil y funcional.

#### Capa de Modelos de Entidad
En esta capa, se concentran los modelos de entidad, que se utilizarán para realizar las transacciones a la base de datos y serán un reflejo de las entidades de base de datos. Se han puesto en un proyecto separado para que puedan ser reutilizados por otros proyectos diferentes de persistencia que pudieran surgir, como un proyecto de acceso a datos para otros tipos de base de datos como Oracle o MySql o InMemmory.
>He reutilizado los modelo sde entidad en la capa dominio, por no sobrecargar innecesariamente de transformaciones modelos que son en el ejemplo completamente iguales. Lo normal en un escenario real, es crear un DTO en la capa dominio que aglomerará todos los datos necesarios de las diferentes entidades implicadas en el caso de uso y los devolverá hacia las capas dependientes superiores para si tratamiento y exposición hacia los frontales.
### Capa Transversal
En esta capa se encuentras proyectos cuyos elementos pueden ser usados a lo largo y ancho de la solución. Se encuentran los siguientes proyectos actualmente
#### Proyecto HostedServices
Contiene la lógica de un servicio auto-hospedado que tendrá la aplicación de backend, que a través de un timer, ejecutará una notificación a los clientes cuando haya un elemento en el inventario que esté expirado. Este proceso se realizará cada minuto notificando por SignalR a los clientes de los elementos expirados.
#### Proyecto MappingProfile
Dicho proyecto contiene los mapeos entre las diferentes entidades de la aplicación. Se usará de manera indistinta a nivel de aplicación, dominio y persistencia.
Para ello he decidido utilizar la librería **AutoMapper**, que aunque penaliza levemente el rendimiento, si permite una abstracción de los mapeos y un facil mantenimiento de los mismos, ya que es capaz de mapear por convención de nombre, ahorrando codificación y simplificando el mantenimiento.
#### Proyecto Models
Son modelos que se usarán de manera transversal en la aplicación, como los modelos de las notificaciones en tiempo real con SignalR.

## Test

En el proyecto he realizado pruebas unitarias e integradas.
Para el proyecto de pruebas unitarias, he utilizado MSTest y una librería gratuita para hacer mocking llamada **Moq**.
Para el proyecto de pruebas integradas, situado en la capa 0.- IntegrationTest, he utilizado Xunit y utillizado el enfoque de base de datos InMemmory, para tener en cada testeo una base de datos limpia para testing. He utilizado para ello el paquete de **EntityFrameworkCor.InMemmory**.
>No he realizado una cobertura de testing del 100%, solo he realizaod algunos test de manera ilustrativa. De cualquier forma, para los test en aplicaciones reales, hay que utilizar métodos selectivos de funcionalidad relevante, ya que un escenario de cobertura del 100% no es viable en ningún caso.

## Configuración de la aplicación

Para arrancar la aplicación, hay que configurarle una base de datos SQL Server.
En el fichero de configuración por entorno Development.json unbicado en el proyecto DistributedServices.API, hay que cambiar la cadena de conexión.
Tras esto, habrá que lanzar las migraciones hacia la base de datos. Dichas migraciones se encuentran en el proyecto de persistencia **Persistence.Database** con el comando Update-database en la consola de administración. También podemos descomentar la línea Context.Database.Migrate() en el fichero Startup.cs del proyecto DistibutedServices.API y arrancar el proyecto. Esto lanzará todas las migraciones pendientes y creará la base de datos si no existe. 
>Esta línea es mejor dejarla comentada posteriormente, para evitar lanzar migraciones de manera descontrolada hacia la base de datos.
