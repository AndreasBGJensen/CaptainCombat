using System.Collections.Generic;


namespace CaptainCombat.Common {

    public struct GlobalId {

        public static readonly GlobalId NULL = new GlobalId(0, 0);

        public uint clientId;
        public uint objectId;

        public GlobalId(uint clientId, uint objectId) {
            this.clientId = clientId;
            this.objectId = objectId;
        }

        public static bool operator ==(GlobalId id1, GlobalId id2) {
            if ((object)id1 == null)
                return (object)id2 == null;
            return id1.Equals(id2);
        }

        public static bool operator !=(GlobalId id1, GlobalId id2) {
            return !(id1 == id2);
        }

        public override bool Equals(object obj) {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var id2 = (GlobalId)obj;
            return (clientId == id2.clientId && objectId == id2.objectId);
        }

        public override int GetHashCode() {
            return clientId.GetHashCode() ^ objectId.GetHashCode();
        }

    }


    public class IdGenerator {

        // 0 is reserved for unknown id
        private uint nextId = 1;

        // List of ids that has been generated, but freed again
        private Queue<uint> readyIds = new Queue<uint>();


        public uint Get() {
            // TODO: Either remove this or come up with better solution
            //if (readyIds.Count > 0)
                //return readyIds.Dequeue();
            return nextId++;
        }

        public void Release(uint id) {
            readyIds.Enqueue(id);
        }

    }

}
