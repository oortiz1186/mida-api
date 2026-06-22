# Integración CONTPAQi

Esta carpeta contendrá la lógica específica para conectarse con CONTPAQi Premium.

Objetivo:

- Mantener separada la lógica de CONTPAQi de los controladores de la API.
- Crear servicios especializados para consultar catálogos, documentos, saldos y reportes.
- Evitar que Soporte MIDA y las apps Android dependan directamente de CONTPAQi.

Estructura sugerida:

```txt
Integrations/Contpaqi
  Dtos
  Models
  Services
```

Primeros servicios sugeridos:

- ContpaqiCustomerService
- ContpaqiProductService
- ContpaqiDocumentService
- ContpaqiReportService
