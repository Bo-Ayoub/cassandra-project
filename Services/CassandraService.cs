using Cassandra;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Threading.Tasks;
using ISession = Cassandra.ISession;

namespace cassandra_app.Services
{
    public class CassandraService
    {
        private readonly ICluster _cluster;
        private readonly ISession _session;

        public CassandraService(IOptions<CassandraSettings> cassandraSettings)
        {
            // Create the Cassandra cluster connection
            _cluster = Cluster.Builder()
                  .AddContactPoint(cassandraSettings.Value.ContactPoints)
                  .WithPort(cassandraSettings.Value.Port)
                  .WithLoadBalancingPolicy(new DefaultLoadBalancingPolicy(cassandraSettings.Value.LocalDataCenter))
                  .Build();

            _session = _cluster.Connect(cassandraSettings.Value.Keyspace);
        }

        // Insert a new task
        public async Task InsertTask(Guid id, string title, string description, bool completed)
        {
           // var query = $"INSERT INTO tasks (id, title, description, completed) VALUES ({id}, '{title}', '{description}', {completed})";
            var query = new SimpleStatement(
        "INSERT INTO tasks (id, title, description, completed) VALUES (?, ?, ?, ?)",
        id, title, description, completed
    );
            await _session.ExecuteAsync(query);
        }

        // Get a task by ID
        // Get a task by ID
        public async Task<Row> GetTaskById(Guid id)
        {
            var query = new SimpleStatement("SELECT * FROM tasks WHERE id = ?", id);
            var result = await _session.ExecuteAsync(query);
            return result.FirstOrDefault(); // Use FirstOrDefault to get the first matching row
        }


        // Get all tasks
        public async Task<List<Row>> GetAllTasks()
        {
            var query = new SimpleStatement("SELECT * FROM tasks");
            var result = await _session.ExecuteAsync(query);
            return result.ToList(); // Convert to List<Row>
        }

        // Delete a task
        public async Task DeleteTask(Guid id)
        {
            var query = new SimpleStatement("DELETE FROM tasks WHERE id = ?", id);
            await _session.ExecuteAsync(query);
        }

        // update a task
        public async Task UpdateTask(Guid id, string title, string description, bool completed)
        {
            var query = @"UPDATE tasks 
                  SET title = ?, description = ?, completed = ? 
                  WHERE id = ?";
            var statement = new SimpleStatement(query, title, description, completed, id);
            await _session.ExecuteAsync(statement);
        }

    }
}
