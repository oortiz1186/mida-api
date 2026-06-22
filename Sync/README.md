# Sincronización

Esta carpeta contendrá la lógica de sincronización entre CONTPAQi Premium, la base intermedia y Soporte MIDA.

Casos previstos:

- Sincronizar clientes / empresas.
- Sincronizar productos o servicios.
- Sincronizar documentos.
- Sincronizar saldos.
- Registrar historial de sincronizaciones.

La idea es que los controladores llamen a servicios de sincronización y no ejecuten reglas de negocio directamente.
