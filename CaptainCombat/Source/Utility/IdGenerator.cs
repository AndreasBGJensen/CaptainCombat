using System.Collections.Generic;


namespace CaptainCombat.Source.Utility {

    public class IdGenerator {

        // 0 is reserved for unknown id
        private uint nextId = 1;

        // List of ids that has been generated, but freed again
        private Queue<uint> readyIds = new Queue<uint>();


        public uint Get() {
            if (readyIds.Count > 0)
                return readyIds.Dequeue();
            return nextId++;
        }

        public void Release(uint id) {
            readyIds.Enqueue(id);
        }

    }

}
