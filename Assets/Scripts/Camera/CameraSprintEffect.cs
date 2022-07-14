using UnityEngine;

[System.Serializable]
public class CameraSprintEffect 
{
	[SerializeField] private ParticleSystem sprintEffect;

	public void SpeedLines(PlayerRef player)
	{
		if (player.PlayerMovement.Magnitude >= 10f)
		{
			if (!sprintEffect.isPlaying) sprintEffect.Play();

			float rateOverLifeTime = Mathf.Max(Vector3.Angle(player.PlayerMovement.Velocity, player.PlayerCam.transform.forward) * 0.15f, 1f);
			rateOverLifeTime = player.PlayerMovement.Magnitude * 2f / rateOverLifeTime;

			ParticleSystem.EmissionModule em = sprintEffect.emission;
			em.rateOverTime = rateOverLifeTime * (player.PlayerMovement.Grounded ? 0.3f : 1.5f);

			ParticleSystem.VelocityOverLifetimeModule velOverLife = sprintEffect.velocityOverLifetime;
			velOverLife.speedModifier = player.PlayerMovement.Magnitude / 20f;
		}
		else if (sprintEffect.isPlaying) sprintEffect.Stop();
	}
}
