## 📋 Board: Dockerización de la Aplicación con SQL Server

### 🏁 Sprint Goal

Dockerizar la aplicación SoftwareDeveloperCase para que funcione con SQL Server en contenedores separados, preparándola para CI/CD con GitHub Actions.

---

### 📊 Columnas del Board

#### 📝 **Backlog** | 🚀 **In Progress** | 🧪 **Testing** | ✅ **Done**

---

### 📌 **Tareas**

#### **🔧 1. Preparación del Entorno Docker** ✅

**Story Points:** 2 | **Priority:** High | **Labels:** `docker`, `setup`

**Descripción:** Configurar el ambiente base para Docker

- [x] Verificar que Docker Desktop está instalado y funcionando
- [x] Revisar el Dockerfile existente en Dockerfile
- [x] Actualizar .dockerignore si es necesario
- [x] Crear estructura de carpetas para scripts Docker

**Acceptance Criteria:**

- ✅ Docker Desktop funcionando en local
- ✅ Dockerfile revisado y optimizado
- ✅ .dockerignore completo

---

#### **🗄️ 2. Configurar SQL Server en Docker**

**Story Points:** 3 | **Priority:** High | **Labels:** `database`, `docker`

**Descripción:** Crear configuración para SQL Server containerizado

- [ ] Crear `docker-compose.yml` con servicio SQL Server
- [ ] Configurar volúmenes para persistencia de datos
- [ ] Definir variables de entorno (SA_PASSWORD, etc.)
- [ ] Agregar health check para SQL Server
- [ ] Crear script de inicialización de BD (opcional)

**Acceptance Criteria:**

- SQL Server se levanta con `docker-compose up`
- Datos persisten entre reinicios
- Health check funcionando

---

#### **🔌 3. Actualizar Connection Strings**

**Story Points:** 2 | **Priority:** High | **Labels:** `configuration`, `backend`

**Descripción:** Modificar configuración para conectar a SQL Server

- [ ] Crear `appsettings.Docker.json`
- [ ] Actualizar connection string para apuntar a contenedor SQL Server
- [ ] Configurar variables de entorno en docker-compose
- [ ] Asegurar que la conexión usa el hostname del servicio Docker

**Acceptance Criteria:**

- Connection string apunta a `sqlserver` (nombre del servicio)
- Configuración separada para Docker
- Variables sensibles en environment

---

#### **🏗️ 4. Configurar API en Docker Compose**

**Story Points:** 3 | **Priority:** High | **Labels:** `api`, `docker`

**Descripción:** Agregar servicio de API al docker-compose

- [ ] Agregar servicio `api` en docker-compose.yml
- [ ] Configurar build context y dockerfile path
- [ ] Establecer dependencia con SQL Server (depends_on)
- [ ] Mapear puertos (8080:8080)
- [ ] Configurar red compartida entre servicios

**Acceptance Criteria:**

- API se construye y ejecuta en Docker
- API espera a que SQL Server esté listo
- Comunicación entre contenedores funciona

---

#### **📊 5. Implementar Migraciones Automáticas**

**Story Points:** 3 | **Priority:** Medium | **Labels:** `database`, `migrations`

**Descripción:** Aplicar migraciones de EF Core al iniciar

- [ ] Modificar `Program.cs` para aplicar migraciones en Docker/Development
- [ ] Agregar manejo de errores para migraciones
- [ ] Implementar retry logic para conexión inicial
- [ ] Agregar logs informativos del proceso

**Acceptance Criteria:**

- Migraciones se aplican automáticamente al iniciar
- Retry logic si la BD no está lista
- Logs claros del proceso

---

#### **🧪 6. Crear Scripts de Utilidad**

**Story Points:** 2 | **Priority:** Medium | **Labels:** `tooling`, `dx`

**Descripción:** Scripts para facilitar el desarrollo

- [ ] Crear `docker-up.sh` para levantar servicios
- [ ] Crear `docker-down.sh` para detener y limpiar
- [ ] Crear `docker-logs.sh` para ver logs
- [ ] Crear `docker-reset.sh` para reset completo
- [ ] Documentar uso en README

**Acceptance Criteria:**

- Scripts funcionan en Mac/Linux
- Scripts documentados
- Manejo de errores básico

---

#### **📝 7. Actualizar Documentación**

**Story Points:** 2 | **Priority:** Medium | **Labels:** `documentation`

**Descripción:** Documentar el nuevo setup Docker

- [ ] Actualizar README.md con instrucciones Docker
- [ ] Crear DOCKER.md con detalles técnicos
- [ ] Documentar troubleshooting común
- [ ] Agregar diagramas de arquitectura Docker
- [ ] Actualizar CHANGELOG.md

**Acceptance Criteria:**

- README incluye quickstart con Docker
- Documentación técnica completa
- Troubleshooting documentado

---

#### **🔐 8. Configurar Secretos y Seguridad**

**Story Points:** 2 | **Priority:** High | **Labels:** `security`

**Descripción:** Asegurar configuración sensible

- [ ] Usar secrets en docker-compose para passwords
- [ ] Crear .env.example con variables necesarias
- [ ] Agregar `.env` a .gitignore
- [ ] Documentar manejo de secretos
- [ ] Validar que no hay credenciales hardcodeadas

**Acceptance Criteria:**

- No hay secretos en el código
- .env.example documentado
- Proceso seguro documentado

---

#### **✅ 9. Testing de Integración Docker**

**Story Points:** 3 | **Priority:** High | **Labels:** `testing`, `integration`

**Descripción:** Probar el sistema completo

- [ ] Probar `docker-compose up` desde cero
- [ ] Verificar que las migraciones se aplican
- [ ] Probar endpoints básicos de la API
- [ ] Verificar persistencia de datos
- [ ] Probar ciclo completo: up, down, up
- [ ] Validar logs y health checks

**Acceptance Criteria:**

- Sistema levanta sin errores
- API responde correctamente
- Datos persisten entre reinicios

---

#### **🚀 10. Optimización del Dockerfile**

**Story Points:** 2 | **Priority:** Low | **Labels:** `performance`, `docker`

**Descripción:** Optimizar imagen Docker

- [ ] Implementar multi-stage build
- [ ] Minimizar capas de imagen
- [ ] Optimizar cache de restauración NuGet
- [ ] Reducir tamaño final de imagen
- [ ] Agregar labels de metadata

**Acceptance Criteria:**

- Imagen < 200MB
- Build time optimizado
- Cache eficiente

---

#### **🔄 11. Preparar para CI/CD**

**Story Points:** 3 | **Priority:** Medium | **Labels:** `ci-cd`, `github-actions`

**Descripción:** Adaptar GitHub Actions para Docker

- [ ] Crear job para build de imagen Docker
- [ ] Configurar push a GitHub Container Registry
- [ ] Agregar tests con docker-compose en CI
- [ ] Configurar cache de Docker layers
- [ ] Documentar proceso de release

**Acceptance Criteria:**

- CI construye imagen Docker
- Tests corren en contenedores
- Imagen se publica en registry

---

#### **📊 12. Monitoreo y Logging**

**Story Points:** 2 | **Priority:** Low | **Labels:** `observability`

**Descripción:** Mejorar observabilidad en Docker

- [ ] Configurar Serilog para Docker
- [ ] Agregar health endpoint extendido
- [ ] Configurar volumen para logs persistentes
- [ ] Agregar container labels para tracking
- [ ] Documentar cómo ver logs

**Acceptance Criteria:**

- Logs estructurados funcionando
- Health check detallado
- Logs accesibles fácilmente

---

### 📈 **Métricas del Sprint**

- **Total Story Points:** 30
- **Duración Estimada:** 1-2 sprints
- **Riesgos Identificados:**
  - Compatibilidad de SQL Server en diferentes OS
  - Tiempo de inicio de contenedores
  - Configuración de red en Docker Desktop

### 🎯 **Definition of Done**

- [ ] Código revisado y aprobado
- [ ] Tests pasando (unit + integration)
- [ ] Documentación actualizada
- [ ] Docker Compose funcionando en Mac/Linux/Windows
- [ ] Sin secretos hardcodeados
- [ ] CHANGELOG.md actualizado
- [ ] README.md con instrucciones claras

### 💡 **Nice to Have** (Futuro)

- Kubernetes manifests
- Helm charts
- Docker Swarm config
- Monitoring con Prometheus/Grafana
- ELK stack para logs
- Backup automatizado de BD

---

### 🏃 **Quick Start Guide**

```bash
# Clonar repo
git clone <repo-url>
cd SoftwareDeveloperCase

# Configurar ambiente
cp .env.example .env
# Editar .env con tus valores

# Levantar servicios
docker-compose up -d

# Ver logs
docker-compose logs -f

# Detener servicios
docker-compose down
```
