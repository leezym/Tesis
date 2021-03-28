Instalar los paquetes
- Firebase.Auth
- Firebase.Firestore

///////////////////////////////////////////////////////////

Para el error "unable find to app directory"

- Abrir el cmd
- Buscar la ruta C:\Users\[username]\AppData\Local
- Escribir "mkdir com.JaverianaCali.YincanaPUJ"

///////////////////////////////////////////////////////////

Inicializar el git (solo se hace la primera vez al clonar)
- git init

-----------------------------------------------------------

Clonar repositorio
- git clone <url repositorio>

-----------------------------------------------------------

Pasarse a rama que ya existe
- git checkout <nombre de la rama>

Crear una rama y subir los cambios
- git checkout -b <nombre de la rama>
- git add .
- git commit -m "<nombre rama>:<mensaje>"
- git push origin <nombre de la rama>

-----------------------------------------------------------

Traer cambios a mi rama de mi propia rama
- git fetch --all
- git pull

Traer cambios a mi rama de otra rama
- git fetch --all
- git pull origin <nombre de la otra rama>

-----------------------------------------------------------

Subir los cambios a la rama
- git add .
- git commit -m "<nombre rama>:<mensaje>"
- git push

-----------------------------------------------------------

RESET (sin push)
git reset --hard HEAD~1
git reset --soft HEAD~1

Nota: Si tienes un código específico de commit, en lugar de HEAD~1 va el código de commit al que quieres devolverte.

-----------------------------------------------------------

REVERT (con push)
git revert HEAD


///////////////////////////////////////////////////////////