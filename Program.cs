using Microsoft.AnalysisServices.AdomdClient; // Para conectar con SSAS y trabajar con CellSet
using System.Collections.Generic; // Para usar List y Dictionary
using Microsoft.AspNetCore.Builder; // Para la configuración del WebApplication
using Microsoft.Extensions.DependencyInjection; // Para el servicio del API explorer
using Microsoft.Extensions.Hosting; // Para el manejo del ambiente (Development, Production)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Conexión a SSAS
var ssasConnectionString = "Data Source=localhost\\TABULAR;Catalog=Tabular_APPSNICA;";

// Transformar el resultado en JSON
List<Dictionary<string, object>> TransformToJSON(CellSet result)
{
    var jsonData = new List<Dictionary<string, object>>();
    int cellIndex = 0;

    foreach (var rowPosition in result.Axes[1].Positions)  // Eje de filas (Dimensiones)
    {
        var dataPoint = new Dictionary<string, object>();

        for (int i = 0; i < rowPosition.Members.Count; i++)
        {
            var dimensionName = result.Axes[1].Set.Hierarchies[i].Name;
            dataPoint[dimensionName] = rowPosition.Members[i].Caption; // Añadir nombre de la dimensión y valor
        }

        for (int colIndex = 0; colIndex < result.Axes[0].Positions.Count; colIndex++)
        {
            var measureName = result.Axes[0].Positions[colIndex].Members[0].Caption; // Captura el nombre de la medida
            var cellValue = result.Cells[cellIndex].Value; // Captura el valor correcto de la celda

            dataPoint[measureName] = cellValue;
            cellIndex++; // Aumentar el índice de la celda
        }

        jsonData.Add(dataPoint);
    }

    return jsonData;
}


// PromedioCategoria endpoint, esta consulta nos muestra el promedio de las categoria que tienen las aplicaciones
app.MapGet("/PromedioCategoria", () =>
{
    using (AdomdConnection connection = new AdomdConnection(ssasConnectionString))
    {
        connection.Open();
        string query = "EVALUATE SUMMARIZECOLUMNS('Aplicaciones'[NombreCategoria], \"Promedio Calificacion por Categoria\", [Promedio Calificacion por Categoria])";
        using (AdomdCommand command = new AdomdCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            var result = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader.GetValue(i));
                }
                result.Add(row);
            }

            return Results.Ok(result);
        }
    }
})
.WithName("PromedioCategoria")
.WithOpenApi();

// Calificacion3y4 endpoint, esta consulta nos muestra las aplicacion con la calificacion entre 3 y 4

app.MapGet("/Calificacion3y4", () =>
{
    using (AdomdConnection connection = new AdomdConnection(ssasConnectionString))
    {
        connection.Open();
        string query = "EVALUATE SUMMARIZECOLUMNS('Aplicaciones'[Nombre], \"CalificacionEntre3y4\", [CalificacionEntre3y4])";
        using (AdomdCommand command = new AdomdCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            var result = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader.GetValue(i));
                }
                result.Add(row);
            }

            return Results.Ok(result);
        }
    }
})
.WithName("Calificacion3y4")
.WithOpenApi();




// CalificacionPositivas endpoint, esta consulta nos muestra las aplicacion con la calificacion entre 4 y 5

app.MapGet("/CalificacionPositivas", () =>
{
    using (AdomdConnection connection = new AdomdConnection(ssasConnectionString))
    {
        connection.Open();
        string query = "EVALUATE SUMMARIZECOLUMNS('Aplicaciones'[Nombre], \"CalificacionPositiva\", [CalificacionPositiva])";
        using (AdomdCommand command = new AdomdCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            var result = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader.GetValue(i));
                }
                result.Add(row);
            }

            return Results.Ok(result);
        }
    }
})
.WithName("CalificacionPositivas")
.WithOpenApi();


// ConteoCalificaciones endpoint, esta consulta nos el conteo de las calificaciones en los meses

app.MapGet("/ConteoCalificaciones", () =>
{
    using (AdomdConnection connection = new AdomdConnection(ssasConnectionString))
    {
        connection.Open();
        string query = "EVALUATE SUMMARIZECOLUMNS('Fecha'[Month], 'Fecha'[MonthName], \"ConteoCalificaciones\", [ConteoCalificaciones])";
        using (AdomdCommand command = new AdomdCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            var result = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader.GetValue(i));
                }
                result.Add(row);
            }

            return Results.Ok(result);
        }
    }
})
.WithName("ConteoCalificaciones")
.WithOpenApi();



// PromedioCalificacionMes endpoint, esta consulta nos muestra el promedio de las calificaciones en los meses del año 2024

app.MapGet("/PromedioCalificacionMes", () =>
{
    using (AdomdConnection connection = new AdomdConnection(ssasConnectionString))
    {
        connection.Open();
        string query = "EVALUATE SUMMARIZECOLUMNS('Fecha'[Year], 'Fecha'[Month], \"PromedioCalificacionMes\", [PromedioCalificacionMes])";
        using (AdomdCommand command = new AdomdCommand(query, connection))
        using (var reader = command.ExecuteReader())
        {
            var result = new List<Dictionary<string, object>>();

            while (reader.Read())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row.Add(reader.GetName(i), reader.GetValue(i));
                }
                result.Add(row);
            }

            return Results.Ok(result);
        }
    }
})
.WithName("PromedioCalificacionMes")
.WithOpenApi();

app.Run();