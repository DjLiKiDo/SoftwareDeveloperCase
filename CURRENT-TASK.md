# CURRENT-TASK: GitHub Copilot Agent Optimization

**Fecha inicio**: 2025-06-01
**Relacionado con**: Mejorar eficiencia del desarrollo con GitHub Copilot Agent

## Contexto
- **Proyecto**: SoftwareDeveloperCase - API REST de gestión de proyectos
- **Arquitectura**: Clean Architecture (.NET 8)
- **Stack**: CQRS + MediatR, EF Core, JWT, FluentValidation, AutoMapper
- **Documentación existente**: Instrucciones detalladas en `.github/instructions/agent.instructions.md`

## Análisis del Estado Actual
- ✅ Instrucciones básicas del agente configuradas
- ✅ Estructura Clean Architecture bien definida
- ✅ Patrones CQRS implementados
- ⚠️ Necesita optimización de instrucciones específicas por dominio
- ⚠️ Falta configuración de indexación del workspace
- ⚠️ Sin prompts reutilizables para tareas comunes

## Plan de Optimización

### Fase 1: Configuración Base del Workspace
- [ ] Optimizar configuraciones de VS Code para Copilot
- [ ] Configurar indexación inteligente del workspace
- [ ] Crear estructura de instrucciones específicas por dominio

### Fase 2: Instrucciones Especializadas
- [ ] Mejorar instrucciones generales del agente
- [ ] Crear instrucciones específicas para testing
- [ ] Crear instrucciones específicas para API endpoints
- [ ] Crear instrucciones específicas para base de datos

### Fase 3: Prompts y Templates Reutilizables
- [ ] Crear prompts para tareas comunes (CRUD, Commands, Queries)
- [ ] Crear templates de código para patrones frecuentes
- [ ] Configurar snippets de VS Code

### Fase 4: Herramientas y Extensiones
- [ ] Configurar extensiones adicionales relevantes
- [ ] Setup de herramientas MCP si aplica
- [ ] Crear scripts de automatización

## Estado Actual
- ✅ Completado: Análisis del proyecto y documentación existente
- ✅ Completado: Creación del plan detallado de optimización
- ⏳ Pendiente: Aprobación del plan por parte del desarrollador
- ⏳ Pendiente: Inicio de implementación según prioridades

## Archivos Creados
- `docs/COPILOT_OPTIMIZATION_PLAN.md` - Plan completo de optimización

## Decisiones Tomadas
- Estructura de 5 fases priorizadas por impacto
- Enfoque en instrucciones especializadas por dominio
- Templates y prompts reutilizables para patrones comunes
- Métricas de éxito cuantificables
