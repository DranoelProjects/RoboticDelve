using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    [SerializeField] GameObject robotPrefab;
    [SerializeField] GameObject scientist;
    bool isRobotAlreadyInstantiate = false;
    bool isVirtualCamFollowingScientist = true;
    bool swapOnCooldown = false;
    CinemachineVirtualCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GameObject.Find("CMVcamScientist").GetComponent<CinemachineVirtualCamera>();
        scientist.GetComponent<PlayerScript>().CanMoove = true;
    }

    public void SwapBetweenRobotAndPlayer()
    {
        if (!swapOnCooldown)
        {
            if (!isRobotAlreadyInstantiate)
            {
                robotPrefab.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                Instantiate(robotPrefab, new Vector3(scientist.transform.localPosition.x, scientist.transform.localPosition.y, 0), Quaternion.identity);
                isRobotAlreadyInstantiate = true;
            }
            GameObject robot = GameObject.FindGameObjectWithTag("Robot");
            if (isVirtualCamFollowingScientist)
            {
                virtualCamera.Follow = robot.transform;
                robot.GetComponent<PlayerScript>().CanMoove = true;
                scientist.GetComponent<PlayerScript>().CanMoove = false;
                isVirtualCamFollowingScientist = false;

                scientist.transform.Find("Healthbar").gameObject.SetActive(false);
                robot.transform.Find("Healthbar").gameObject.SetActive(true);

            }
            else
            {
                virtualCamera.Follow = scientist.transform;
                robot.GetComponent<PlayerScript>().CanMoove = false;
                scientist.GetComponent<PlayerScript>().CanMoove = true;
                isVirtualCamFollowingScientist = true;

                scientist.transform.Find("Healthbar").gameObject.SetActive(true);
                robot.transform.Find("Healthbar").gameObject.SetActive(false);
            }
            StartCoroutine(SwapCooldown());
        }
    }

    IEnumerator SwapCooldown()
    {
        swapOnCooldown = true;
        yield return new WaitForSeconds(2f);
        swapOnCooldown = false;
    }
}
