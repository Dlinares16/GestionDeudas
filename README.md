# GestionDeudas
Prueba Tecnica Daniel Stiven Linares Hernandez DOUBLE V PARTNERS NYX

Decisiones Técnicas

Backend
Clean Architecture: Separación clara de responsabilidades en capas:

API: Controladores y configuración
BLL: Lógica de negocio y validaciones
DAL: Acceso a datos y repositorios
Model: Entidades del dominio
DTO: Objetos de transferencia para API

JWT + Refresh Tokens:

Seguridad robusta sin almacenamiento servidor
Escalabilidad horizontal
Control granular de expiración

//////////////////////////////////////////////////////////////////////////

Frontend
React + TypeScript:

Type safety en desarrollo
Mejor experiencia de desarrollador
Detección temprana de errores

Redux Toolkit:

Estado global predecible
DevTools excelentes para debugging
RTK Query para manejo eficiente de caché cliente

Vite:

Desarrollo ultra rápido con HMR
Build optimizado
Configuración mínima

/////////////////////////////////////////////////////////////////////////

Instrucciones de despliegue local:

PostgreSQL
Crear la db gestionDeudas y ejecutar el script de creacion de tablas, index, triggers, etc
En la cadena de conexion del back indicar usuario y clave de la db

Backend
En la rama main ejecutar el backend y verificar en que puerto esta corriendo para colocar en el appclient del front

Frontend
Usar npm run dev y verificar en que puerto esta corriendo para colocar en el program.cs del back
