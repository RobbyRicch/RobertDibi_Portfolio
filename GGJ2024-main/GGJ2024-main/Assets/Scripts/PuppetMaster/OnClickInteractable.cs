using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickInteractable : MonoBehaviour, Iinteractable
{
    [SerializeField] float _speed;
    [SerializeField] float _cdTimer = 2f;
    [SerializeField] PuppetMasterController _puppetMaster;
    [SerializeField] ParticleSystem _sparkPS;
    [SerializeField] ParticleSystem _trailPS;
    [SerializeField] ParticleSystem _lightDestruction;


    bool _clicked;

    private void Awake()
    {
        _puppetMaster = GameObject.FindObjectOfType<PuppetMasterController>();
    }

    public void OnInteract()
    {
        _sparkPS.Stop();
        _clicked = true;
        _trailPS.Play();
    }

    public void OnRelease()
    {
        InteractCD();
    }

    public void InteractCD()
    {
        StartCoroutine(InteractCDCor());
    }

    // Update is called once per frame
    void Update()
    {
        if (_clicked) 
        {
            _puppetMaster._canHit = false;
            transform.Translate(Vector3.down * _speed * Time.deltaTime); 
            Destroy(gameObject, 1);
        }
        
    }

    IEnumerator InteractCDCor()
    {
        float timer = _cdTimer;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        _puppetMaster._canInteract = true;
        _puppetMaster._canHit = true;

    }


    private void OnDestroy()
    {
        Debug.Log("Peepee");
        Instantiate(_lightDestruction.gameObject, transform.position, transform.rotation);

    }
}