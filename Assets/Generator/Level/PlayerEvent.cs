using UnityEngine;

public class PlayerEvent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject player = collision.gameObject;
        if (player != null && player.CompareTag("Player"))
        {
            // Select a event by random
            int selector = Random.Range(0, 100);

            if (selector < 20)
            {
                // Change the cool down of player's bullet
                player.GetComponent<PrefabWeapon>().coolDown += Random.Range(-0.25f, 0.25f);
                if (player.GetComponent<PrefabWeapon>().coolDown < 0f)
                {
                    player.GetComponent<PrefabWeapon>().coolDown = 0f;
                }
            }
            else if (selector >= 20 && selector < 40)
            {
                // Change player's running speed
                player.GetComponent<PlayerMovement>().runSpeed += Random.Range(-5f, 5f);
                if (player.GetComponent<PlayerMovement>().runSpeed < 0.5f)
                {
                    player.GetComponent<PlayerMovement>().runSpeed = 0.5f;
                }
            }
            else if (selector >= 40 && selector < 60)
            {
                // Change player's jump force
                player.GetComponent<CharacterController2D>().changeJumpForce(Random.Range(-100f, 100f));
            }
            else if (selector >= 60 && selector < 80)
            {
                // Change the speed of player's bullet
                player.GetComponent<PrefabWeapon>().fireBallSpeed += Random.Range(-5f, 5f);
                if (player.GetComponent<PrefabWeapon>().fireBallSpeed < 0.5f)
                {
                    player.GetComponent<PrefabWeapon>().fireBallSpeed = 0.5f;
                }
            }
            else if (selector >= 80 && selector < 100)
            {
                // Change the damage of player's bullet
                player.GetComponent<PrefabWeapon>().fireBallDamage += Random.Range(-5, 5);
                if (player.GetComponent<PrefabWeapon>().fireBallDamage < 0)
                {
                    player.GetComponent<PrefabWeapon>().fireBallDamage = 0;
                }
            }
            Destroy(gameObject);
        }
    }
}
