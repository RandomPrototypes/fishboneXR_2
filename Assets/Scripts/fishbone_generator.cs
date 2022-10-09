using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fishbone_generator : MonoBehaviour
{
    [SerializeField]
    public GameObject cylinderPrefab;

    [SerializeField]
    public float cylinder_dist = 0.5f;

    [SerializeField]
    public float RotationSpeed = 360.0f/6;

    [SerializeField]
    public int patternId = 0;

    [SerializeField]
    public Material mainCylinderMaterial;

    GameObject[] main_cylinder = new GameObject[8];

    bool rotating = true;
    float rotationStep = 0;

    float initial_offset = 0;

    Vector3 initial_local_pos;
    public float initial_cylinder_dist;

    float main_cylinder_radius = 0.4f;

    float speedRatio = 1.0f;

    bool useCollision = true;

    public int[,] cylinder_config;

    public int[,] cylinder_config1 = new int[7,8] {{1,0,0,1,0,1,0,0},
                                                {0,0,1,0,1,0,1,0},
                                                {1,0,1,0,0,1,0,0},
                                                {1,0,0,1,0,0,1,0},
                                                {0,1,0,0,1,0,0,1},
                                                {1,0,1,0,0,0,1,0},
                                                {0,1,0,1,0,1,0,1}};
    public int[,] cylinder_config2 = new int[3,8] {{0,1,0,0,0,1,0,1},
                                                   {1,0,1,0,0,0,1,0},
                                                   {0,1,0,0,0,1,0,1}};

    public GameObject[,] list_cylinders;
    // Awake is called before the first frame update
    void Awake()
    {
        initial_local_pos = transform.localPosition;
        initial_cylinder_dist = cylinder_dist;

        if(patternId == 0)
            cylinder_config = cylinder_config1;
        else cylinder_config = cylinder_config2;

        list_cylinders = new GameObject[cylinder_config.GetLength(0), cylinder_config.GetLength(1)];

        initial_offset = transform.localPosition.z;
        Debug.Log("initial_offset"+initial_offset);

        float length = cylinder_config.GetLength(0)*cylinder_dist;
        for(int i = 0; i < 8; i++) {
            main_cylinder[i] = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            main_cylinder[i].transform.SetParent(transform);
            main_cylinder[i].transform.localRotation = Quaternion.Euler(new Vector3(90, 0, 0));
            main_cylinder[i].transform.localPosition = Quaternion.Euler(new Vector3(0, 0, (i+0.5f)*360/8)) * new Vector3(0.0f, main_cylinder_radius, length/2 - 0.25f);
            main_cylinder[i].transform.localScale = new Vector3(0.1f,length/2,0.1f);
            main_cylinder[i].GetComponent<Renderer>().material = mainCylinderMaterial;
        }

        for(int i = 0; i < cylinder_config.GetLength(0); i++){
            for(int j = 0; j < cylinder_config.GetLength(1); j++){
                if(cylinder_config[i,j] == 1)
                    list_cylinders[i,j] = createCylinderAt(i,j);
            }
        }
    }

    public GameObject createCylinderAt(int i, int j)
    {
        GameObject cylinder = Instantiate(cylinderPrefab, transform);
        cylinder.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -j*360/8));
        cylinder.transform.localPosition = cylinder.transform.localRotation * new Vector3(0,main_cylinder_radius,i*cylinder_dist);
        return cylinder;
    }

    public void updateCylinderDist(float new_cylinder_dist)
    {
        cylinder_dist = new_cylinder_dist;
    }

    // Update is called once per frame
    void Update()
    {
        //if(list_cylinders == null)
            //return ;
        
        transform.localPosition = new Vector3(initial_local_pos.x, initial_local_pos.y, initial_local_pos.z * cylinder_dist / initial_cylinder_dist);

        for(int i = 0; i < cylinder_config.GetLength(0); i++){
            for(int j = 0; j < cylinder_config.GetLength(1); j++){
                if(cylinder_config[i,j] == 1) {
                    list_cylinders[i,j].transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -j*360/8));
                    list_cylinders[i,j].transform.localPosition = list_cylinders[i,j].transform.localRotation * new Vector3(0,main_cylinder_radius,i*cylinder_dist);
                }
            }
        }

        float length = cylinder_config.GetLength(0)*cylinder_dist;
        for(int i = 0; i < 8; i++) {
            main_cylinder[i].transform.localPosition = Quaternion.Euler(new Vector3(0, 0, (i+0.5f)*360/8)) * new Vector3(0.0f, main_cylinder_radius, length/2 - 0.25f);
            main_cylinder[i].transform.localScale = new Vector3(0.1f,length/2,0.1f);
        }

        if(rotating)
            rotationStep += speedRatio * RotationSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(Vector3.forward * rotationStep);
    }

    public void setRotating(bool val) {
        rotating = val;
    }

    public void setRotationStep(float val)
    {
        rotationStep = val;
    }

    public void setSpeedRatio(float ratio) {
        speedRatio = ratio;
    }

    public void setOffsetPos(int val) {
        Vector3 pos = transform.localPosition;
        transform.localPosition = new Vector3(pos.x, pos.y, initial_offset - val*cylinder_dist);
    }

    public bool getVatAt(int pos, int rotationPos) {
        Debug.Log("pattern "+patternId+" pos "+pos+" rotation "+rotationPos);
        Debug.Log("val "+cylinder_config[pos, rotationPos]);
        return cylinder_config[pos, rotationPos] == 1;
    }

    public void setCollision(bool state) {
        useCollision = state;
    }

    public void setValAt(int pos, int rotationPos, bool val) {
        if(val && cylinder_config[pos, rotationPos] == 0) {
            list_cylinders[pos, rotationPos] = createCylinderAt(pos, rotationPos);
            cylinder_config[pos, rotationPos] = 1;
        }
        else if(!val && cylinder_config[pos, rotationPos] == 1) {
            Destroy(list_cylinders[pos, rotationPos]);
            list_cylinders[pos, rotationPos] = null;
            cylinder_config[pos, rotationPos] = 0;
        }
    }

    public void clearFishbone() {
        for(int i = 0; i < cylinder_config.GetLength(0); i++){
            for(int j = 0; j < cylinder_config.GetLength(1); j++){
                if(cylinder_config[i, j] == 1){
                    Destroy(list_cylinders[i, j]);
                    list_cylinders[i, j] = null;
                    cylinder_config[i, j] = 0;
                }
            }
        }
    }
}
