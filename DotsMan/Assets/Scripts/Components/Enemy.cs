using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable, GenerateAuthoringComponent]
public struct Enemy : IComponentData
{
    // Add fields to your component here. Remember that:
    //
    // * A component itself is for storing data and doesn't 'do' anything.
    //
    // * To act on the data, you will need a System.
    //
    // * Data in a component must be blittable, which means a component can
    //   only contain fields which are primitive types or other blittable
    //   structs; they cannot contain references to classes.
    //
    // * You should focus on the data structure that makes the most sense
    //   for runtime use here. Authoring Components will be used for 
    //   authoring the data in the Editor.


    //public bool isInit;
    public float3 lastCell;
    public float3 lastPos;
    //public double lastMoveChoiceTime;
    //public float decisionSpeed;

	//public void init()
	//{
 //       //lastCell = new float3(-1f, -1f, -1f)
 //       isInit = true;
 //       decisionSpeed = 3;
	//}


}
