## Migración
En mi experiencia migrando sistemas de vb6 a C# .Net realizo la siguientes consideraciones
1. Entender el proceso, como se realiza la operación, que actores intervienen, que lógica de negocio manejan (todo esto sin tocar codigo aún).
2. Una vez entendiendo el proceso, se identifica dentro del código los componentes y/o bloques de codigo que realizan cada operación, función y demás. 
3. Puede ocuparse Ingeniería Inversa para codificar buscando optimizar el código/proceso o bien ir traduciendo la sintaxis del lenguage legacy al nuevo lenguaje, siempre finalizando en los mismos llamados a la base de datos.
4. Pruebas en un ambiente de desarrollo generando peticiones desde sistema legacy y el nuevo sistema para comparar comportamiento y respuestas.
5. Liberación modular para monitorear el comportamiento en producción. 
