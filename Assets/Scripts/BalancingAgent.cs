using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BalancingAgent : Agent
{
    public Transform platform; // Ссылка на платформу
    public Transform ball; // Ссылка на объект, который нужно удерживать

    public GameObject balanced_ball;
    public Rigidbody rgbodybalanced_ball;

    public override void Initialize()
    {
    	rgbodybalanced_ball = balanced_ball.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
    	//Ставим рандомные позиции для шара и платформе при запуске эпохи
    	Vector3 platformPosition = platform.position;
    	platform.rotation = Quaternion.Euler(Random.Range(-30f,30f), 0f, Random.Range(-30f,30f));
    	ball.position = new Vector3(platformPosition.x+Random.Range(-2f,2f), 4f, platformPosition.z+Random.Range(-2f,2f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
    	sensor.AddObservation(rgbodybalanced_ball.velocity);
    	sensor.AddObservation(ball.position);
    	sensor.AddObservation(platform.position);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
    	//Управление платформой
    	float horizontalInput = actionBuffers.ContinuousActions[0];
    	float verticalInput = actionBuffers.ContinuousActions[1];
    	
    	float x = 2 * Mathf.Clamp(horizontalInput, -1, 1);
    	float z = 2 * Mathf.Clamp(verticalInput, -1, 1);

    	//Применение вращения к платформе
    	// platform.Rotate(horizontalInput*Time.deltaTime*100f, verticalInput*Time.deltaTime*100f, forwardInput*Time.deltaTime*100f);
    	platform.Rotate(Vector3.right, z);
    	platform.Rotate(Vector3.forward, -x);

    	//Расчет расстояния между шаром и центром платформы
    	float distance = Vector3.Distance(ball.position, platform.position);
    	float maxdistance = 8.5f;
    	if (distance > maxdistance)
    	{
    		SetReward(-1.0f);
    		EndEpisode();
    	}
    	else
    	{
    		SetReward(0.1f);
    	}
    }

    //Функционал для ручного управления
    public override void Heuristic(in ActionBuffers actionsOut)
    {
    	var continuousActionsOut = actionsOut.ContinuousActions;
    	continuousActionsOut[0] = Input.GetAxis("Horizontal");
    	continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
}
