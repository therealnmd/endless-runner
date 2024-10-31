using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    //sử dụng Transform để có thể kéo thả các đối tượng 
    [SerializeField] private Transform[] levelPart;
    [SerializeField] private Vector3 nextPartPosition;
    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDelete;
    [SerializeField] private Transform player;

    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DeletePlatform();
        GeneratePlatform();
    }

    private void GeneratePlatform()
    {
        while (Vector2.Distance(player.transform.position, nextPartPosition) < distanceToSpawn)
        {
            Transform part = levelPart[Random.Range(0, levelPart.Length)];

            //trục Y sẽ luôn = 0, tránh trường hợp platform spawn ra sẽ bị lệch trục Y
            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("Start Point").position.x, 0);

            //transform ở cuối được xđ là đối tượng cha của newPart
            //tức newPart sẽ trở thành con của đối tượng mà script đang gắn vào (ở đây là LevelGenerator)
            //các platform sau khi nhấn chuột trái sẽ spawn ở trong và ngay dưới LevelGenerator
            Transform newPart = Instantiate(part, newPosition, transform.rotation, transform);
            //nextPartPosition - part.Find("Start Point").position là để mỗi khi click chuột trái
            //thì sẽ generate ra 1 platform mới có điểm xuất phát nằm chính xác ở Start Point
            nextPartPosition = newPart.Find("End Point").position;
        }
    }
    private void DeletePlatform()
    {
        if (transform.childCount > 0)
        {
            //lấy platform đầu tiên làm partToDelete
            Transform partToDelete = transform.GetChild(0);
            //khi mà khoảng cách giữa player và partToDelete dài hơn khoảng cách cần xoá (=50) thì xoá cái đầu
            if (Vector2.Distance(player.transform.position, partToDelete.transform.position) > distanceToDelete)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }
}
