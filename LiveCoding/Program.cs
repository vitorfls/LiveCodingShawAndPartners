// All the code should be in this file and you won't need to use any external library
// You can run the code as many times as you want
// If you have any question please ask the interviewer instead of searching on the internet
// Good Test!
using System;
using System.Collections.Generic;
using System.Linq;

public class Column
{
    public string Type { get; set; }
    public bool PrimaryKey { get; set; }
    public bool AutoIncrement { get; set; }
    public bool NotNull { get; set; }
}

public class ORM
{

    // Put your code here!
    // Other classes can be added outside this one.

    public Dictionary<string, Table> _tables = new Dictionary<string, Table>();

    public Table Table(string tableName)
    {
        return _tables[tableName];
    }

    public Dictionary<string, Column> CreateTable(string TableName, Dictionary<string, Column> Columns)
    {

        Table table = new Table(TableName, Columns);

        _tables[TableName] = table;

        Dictionary<string, Column> record = new Dictionary<string, Column>
        {
            { TableName, new Column() }
        };

        return record;
    }


    public string GetTables()
    {
        return String.Join(" ", _tables.Keys);
    }
}

public class Table
{

    private Dictionary<string, Column> _columns = new Dictionary<string, Column>();
    private List<Dictionary<string, object>> _rows = new List<Dictionary<string, object>>();
    private static int _id = 0;

    public string TableName { get; }

    public Table(string tableName, Dictionary<string, Column> columns)
    {
        this.TableName = tableName;
        this._columns = columns;
    }

    public Dictionary<string, object> Insert(Dictionary<string, object> values)
    {

        // Validar se os values estão dentro do schema
        if (!ValidateColumns(values))
        {
            return null;
        }

        // Criar um registro vazio com valores padrão baseados nas especificações das colunas
        Dictionary<string, object> emptyRecord = CreateCloneRecord();

        foreach (var kvp in values)
        {
            string key = kvp.Key;
            object value = kvp.Value;

            if (emptyRecord.ContainsKey(key))
            {
                // Atualize a chave correspondente no emptyRecord com o novo valor
                emptyRecord[key] = value;
            }
        }

        // Validar colunas
        if (ValidateRow(emptyRecord))
        {
            // Se o registro passar na validação, inserir no repositório
            _rows.Add(emptyRecord);
            return _rows.LastOrDefault();
        }
        else
        {
            // Em caso de erro na validação, fazer o Rollback do ID (Chave-primária)
            _id = _id - 1;
        }

        // TODO: lançar uma exceção
        return null;
    }

    public Dictionary<string, object> Update(int id, Dictionary<string, object> values)
    {

        // Valida se os values estão dentro do schema
        if (!ValidateColumns(values))
        {
            return null;
        }

        // Obter o índice a ser atualizado
        int indexToUpdate = GetIndexById(id);

        // Criar uma cópia em memória do registro a ser atualizado
        Dictionary<string, object> updateRecord = new Dictionary<string, object>(_rows[indexToUpdate]);

        // Valor a copia
        foreach (var kvp in values)
        {
            string key = kvp.Key;
            object value = kvp.Value;

            if (updateRecord.ContainsKey(key))
            {
                // Atualizar a chave correspondente com o novo valor
                updateRecord[key] = value;
            }
        }

        // Validar o registro atualizado
        if (ValidateRow(updateRecord))
        {
            // Se o registro atualizado passar na validação, atualizar repositório
            _rows[indexToUpdate] = updateRecord;
            return updateRecord;
        }

        // TODO: lançar uma exceção
        return null;
    }

    public Dictionary<string, object> DeleteById(int id)
    {
        // Encontrar o índice do registro a ser excluído
        int indexToDelete = GetIndexById(id);

        // Criar uma cópia do registro a ser excluído (Para ser retornado)
        Dictionary<string, object> deletedKey = new Dictionary<string, object>(_rows[indexToDelete]);

        // Remover o registro do repositório
        _rows.RemoveAt(indexToDelete);

        // Retorne o registro excluído
        return deletedKey;
    }

    public List<Dictionary<string, object>> GetAll()
    {
        return _rows;
    }
    public Dictionary<string, object> CreateCloneRecord()
    {
        // Criar um novo registro com valores padrão com base nas especificações da coluna
        Dictionary<string, object> record = new Dictionary<string, object>();

        foreach (var column in _columns)
        {
            string columnName = column.Key;
            Column columnSpec = column.Value;

            if (columnSpec.Type == "integer")
            {
                // Definir o valor padrão para colunas do tipo inteiro
                record[columnName] = 0;

                if (columnSpec.AutoIncrement)
                {
                    // Se for AutoIncrement
                    if (columnSpec.PrimaryKey)
                    {
                        // Se for uma chave primária AutoIncrement
                        _id = _id + 1;
                        record[columnName] = _id; // Valor padrão para chave primária com auto-incremento
                    }
                }
            }
            else if (columnSpec.Type == "text")
            {
                // Definir o valor padrão para colunas texto
                record[columnName] = string.Empty;
            }
            // Você pode adicionar condições adicionais para outros tipos de dados, conforme necessário
        }

        return record;
    }


    private bool ValidateColumns(Dictionary<string, object> input)
    {
        // Validar schema
        foreach (var pvc in input)
        {
            if (!_columns.ContainsKey(pvc.Key))
            {
                Console.WriteLine($"{pvc.Key} doesnt exists on schema");
                return false;
            }
        }

        // Todas as validações foram bem-sucedidas
        return true;
    }


    private bool ValidateRow(Dictionary<string, object> input)
    {

        // Validar o valor de cada coluna em relação às suas especificações
        foreach (var col in _columns)
        {
            string columnName = col.Key; // Obter o identificador da coluna
            Column columnSpec = _columns[columnName]; // Obter as especificações da coluna (not null, tipo, etc)

            if (!ValidateRowValue(columnName, input, columnSpec))
            {
                // O valor não corresponde às especificações da coluna
                return false;
            }
        }

        // Todas as validações foram bem-sucedidas
        return true;
    }


    private bool ValidateRowValue(string columnName, Dictionary<string, object> values, Column columnSpec)
    {
        // A coluna foi fornecida; 
        string kpv = values[columnName].ToString();

        // Verificar se a coluna está vazia
        if (String.IsNullOrEmpty(kpv))
        {
            // Se a coluna for especificada como not null, ela é inválida
            if (columnSpec.NotNull)
            {
                Console.WriteLine($"{columnName} cannot be null");
                return false;
            }
        }
        else
        {
            if (columnSpec.Type == "integer")
            {
                // Tente fazer o parsing do valor como int
                if (!int.TryParse(kpv, out int intValue))
                {
                    // A string não representa um int
                    Console.WriteLine($"{columnName} must be a int");
                    return false;
                }
            }

            if (columnSpec.Type == "text")
            {
                // Tente fazer o parsing do valor como um inteiro
                if (int.TryParse(kpv, out int intValue))
                {
                    // A string representa um inteiro, e é inválida, pois deveria ser text
                    Console.WriteLine($"{columnName} must be a string");
                    return false;
                }
            }
        }

        // Todas as validações foram bem-sucedidas
        return true;
    }


    public int GetIndexById(int id)
    {
        return _rows.FindIndex(dict => dict.ContainsKey("id") && dict["id"] is int && (int)dict["id"] == id);
    }

}



class Program
{
    static void Main(string[] args)
    {
        ORM orm = new ORM();

        orm.CreateTable("users", new Dictionary<string, Column>
        {
            { "id", new Column { Type = "integer", PrimaryKey = true, AutoIncrement = true } },
            { "name", new Column { Type = "text", NotNull = true } },
            { "email", new Column { Type = "text", NotNull = true } }
        });

        orm.CreateTable("messages", new Dictionary<string, Column>
        {
            { "id", new Column { Type = "integer", PrimaryKey = true, AutoIncrement = true } },
            { "title", new Column { Type = "text", NotNull = true } },
            { "description", new Column { Type = "text", NotNull = true } }
        });


        Console.WriteLine("getTables: " + orm.GetTables());

        // // getTables: users, messages

        Console.WriteLine("insert:");

        PrintDictionary(orm.Table("users").Insert(
        new Dictionary<string, object>
        {
                { "name", "John Doe" },
                { "email", "john.doe@email.com" }
        }));


        // /*
        //  insert:
        //  --------------------
        //  id: 1
        //  name: John Doe
        //  email: john.doe@email.com
        // */

        Console.WriteLine("insert:");
        PrintDictionary(orm.Table("users").Insert(
            new Dictionary<string, object>
            {
                 { "name", "Jane Doe" },
                 { "email", "jane.doe@email.com" }
            }));
        // /*
        //  insert:
        //  --------------------
        //  id: 2
        //  name: Jane Doe
        //  email: jane.doe@email.com
        // */


        Console.WriteLine("getAll:");
        foreach (var element in orm.Table("users").GetAll())
            PrintDictionary(element);
        PrintSpacer();
        // /*
        //  getAll:
        //  --------------------
        //  id: 1
        //  name: John Doe
        //  email: john.doe@email.com
        //  --------------------
        //  id: 2
        //  name: Jane Doe
        //  email: jane.doe@email.com
        //  --------------------
        // */

        Console.WriteLine("delete:");
        PrintDictionary(orm.Table("users").DeleteById(2));

        // /*
        //  delete:
        //  --------------------
        //  id: 2
        //  name: Jane Doe
        //  email: jane.doe@email.com
        //  */


        Console.WriteLine("getAll:");
        foreach (var element in orm.Table("users").GetAll())
            PrintDictionary(element);
        PrintSpacer();
        // /*
        //  getAll:
        //  --------------------
        //  id: 1
        //  name: John Doe
        //  email: john.doe@email.com
        //  --------------------
        // */

        Console.WriteLine("insert:");
        PrintDictionary(orm.Table("users").Insert(
            new Dictionary<string, object>
            {
                 { "name", "Joseph Doe" },
                 { "email", "joseph.doe@email.com" }
            }));

        Console.WriteLine("getAll:");
        foreach (var element in orm.Table("users").GetAll())
            PrintDictionary(element);
        PrintSpacer();
        // /*
        //  getAll:
        //  --------------------
        //  id: 1
        //  name: John Doe
        //  email: john.doe@email.com
        //  --------------------
        //  id: 3
        //  name: Joseph Doe
        //  email: joseph.doe@email.com
        //  --------------------
        // */

        Console.WriteLine("update:");
        PrintDictionary(orm.Table("users").Update(1,
            new Dictionary<string, object>
            {
                 { "name", "John Doe Updated" }
            }));

        Console.WriteLine("getAll:");
        foreach (var element in orm.Table("users").GetAll())
            PrintDictionary(element);
        PrintSpacer();
        // /*
        //  getAll:
        //  --------------------
        //  id: 1
        //  name: John Doe Updated
        //  email: john.doe@email.com
        //  --------------------
        //  id: 3
        //  name: Joseph Doe
        //  email: joseph.doe@email.com
        //  --------------------
        // */

        // /*
        //  * *************
        //  * *** BONUS ***
        //  * *************
        //  */

        Console.WriteLine("\n ** BONUS ** \n");

        Console.Write("insert: ");
        orm.Table("messages").Insert(
            new Dictionary<string, object>
            {
                 { "description", "A message description" }
            });
        // insert: Invalid: title cannot be null

        Console.Write("insert: ");
        orm.Table("users").Insert(
            new Dictionary<string, object>
            {
                 { "name", "John Doe" }
            });
        // // insert: Invalid: email cannot be null

        Console.Write("insert: ");
        orm.Table("users").Insert(
            new Dictionary<string, object>
            {
                 { "name", 10 },
                 { "email", "john.doe@email.com" }
            });
        // insert: Invalid: name must be a string

        Console.Write("update: ");
        orm.Table("users").Update(3, new Dictionary<string, object> { { "name", 10 } });
        // update: Invalid: name must be a string

        Console.Write("update: ");
        orm.Table("users").Update(3,
            new Dictionary<string, object>
            {
                 { "name", "Other" }, { "lastName", "Person" }
            });
        // update: Invalid: lastName doesnt exists on schema

        Console.Write("insert: ");
        orm.Table("messages").Insert(new Dictionary<string, object> { { "name", "John Doe" } });
        // insert: Invalid: name doesnt exists on schema



    }

    private static void PrintDictionary(KeyValuePair<string, object> element)
    {
        throw new NotImplementedException();
    }

    private static void PrintSpacer()
    {
        Console.WriteLine("--------------------");
    }

    private static void PrintDictionary(Dictionary<string, object> dictionary)
    {
        PrintSpacer();
        foreach (var (key, value) in dictionary)
        {
            Console.WriteLine($"{key}: {value}");
        }
    }
}