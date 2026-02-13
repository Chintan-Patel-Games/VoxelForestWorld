// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 4.0.11
// 

using Colyseus.Schema;
#if UNITY_5_3_OR_NEWER
using UnityEngine.Scripting;
#endif

namespace ColyseusSchema {
	public partial class Player : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public Player() { }
		[Type(0, "number")]
		public float x = default(float);

		[Type(1, "number")]
		public float y = default(float);

		[Type(2, "number")]
		public float z = default(float);

		[Type(3, "number")]
		public float rotY = default(float);

		[Type(4, "number")]
		public float lookX = default(float);

		[Type(5, "number")]
		public float velY = default(float);

		[Type(6, "number")]
		public float inputX = default(float);

		[Type(7, "number")]
		public float inputZ = default(float);

		[Type(8, "boolean")]
		public bool grounded = default(bool);
	}
}
