using Microsoft.Xna.Framework;
using PixelFactory.Entities;
using PixelFactory.Inventory;
using PixelFactory.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelFactory.Crafting
{
    public class Crafter : Entity
    {
        public bool HasOutputItems { get => Outputs.Count > 0; }
        public bool HasPendingJobs { get => PendingJobs.Count > 0; }
        public bool HasActiveJobs { get => ActiveJobs.Count > 0; }
        public int ParallelJobs { get; private set; } = 1;
        public List<CraftingJob> ActiveJobs { get; private set; }
        public Queue<InventorySlot> Outputs { get; private set; }
        public Queue<CraftingJob> PendingJobs { get; private set; }
        public Crafter()
        {
            ActiveJobs = new List<CraftingJob>();
            Outputs = new Queue<InventorySlot>();
            PendingJobs = new Queue<CraftingJob>();
        }
        public Crafter(int parallelJobs)
        {
            Outputs = new Queue<InventorySlot>();
            PendingJobs = new Queue<CraftingJob>();
            ActiveJobs = new List<CraftingJob>();
            ParallelJobs = parallelJobs;
        }
        public Crafter(Recipe recipe, int count = 1, int parallelJobs = 1)
        {
            Outputs = new Queue<InventorySlot>();
            PendingJobs = new Queue<CraftingJob>();
            ActiveJobs = new List<CraftingJob>();
            ParallelJobs = parallelJobs;
            Enqueue(recipe, count);
        }
        public void Enqueue(Recipe recipe, int count = 1)
        {
            for (int i = 0; i < count; ++i)
            {
                PendingJobs.Enqueue(new CraftingJob(recipe));
            }
        }

        public InventorySlot DequeueOutput()
        {
            if (Outputs.Count == 0)
                return null;
            return Outputs.Dequeue();
        }
        public void RemoveFirstOutput()
        {
            DequeueOutput();
        }
        public InventorySlot GetFirstOutput()
        {
            return Outputs.Peek();
        }
        public List<InventorySlot> GetOutputs()
        {
            List<InventorySlot> outputs = new List<InventorySlot>();
            while (HasOutputItems)
            {
                outputs.Add(DequeueOutput());
            }
            return outputs;
        }
        private bool CanAcceptActiveJobs { get => ActiveJobs.Count <= ParallelJobs; }
        public override void Update(GameTime gameTime)
        {

            if (!HasPendingJobs && !HasActiveJobs)
                return;

            List<CraftingJob> toRemove = new List<CraftingJob>();
            foreach (var activeJob in ActiveJobs)
            {
                activeJob.Update(gameTime);
                if (activeJob.Finished)
                {
                    toRemove.Add(activeJob);
                }
            }
            foreach (var job in toRemove)
            {
                while (job.HasOutputItems)
                {
                    Outputs.Enqueue(job.GetOutput());
                }
                ActiveJobs.Remove(job);
            }
            if (HasPendingJobs && CanAcceptActiveJobs)
            {
                for (int i = 0; i < ParallelJobs - ActiveJobs.Count; ++i)
                {
                    ActiveJobs.Add(PendingJobs.Dequeue());
                }
            }
            base.Update(gameTime);
        }
        public override List<byte> GetData()
        {
            List<byte> data = base.GetData();
            Serializer.WriteInt(ActiveJobs.Count, data);
            for (int i = 0; i < ActiveJobs.Count; ++i)
            {
                data.AddRange(ActiveJobs.ElementAt(i).GetData());
            }
            Serializer.WriteInt(PendingJobs.Count, data);
            for (int i = 0; i < PendingJobs.Count; ++i)
            {
                data.AddRange(PendingJobs.ElementAt(i).GetData());
            }
            Serializer.WriteInt(Outputs.Count, data);
            for (int i = 0; i < Outputs.Count; ++i)
            {
                data.AddRange(Outputs.ElementAt(i).GetData());
            }
                return data;
        }
    }
}
