

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Bronie : MonoBehaviour
{
  [SerializeField]
  protected ParticleSystem Wystrzal;
  [SerializeField]
  protected GameObject impactEffect;
  [SerializeField]
  protected Camera fpscamera;
  [SerializeField]
  protected Animator anim;
  private Wynik wynik;
  [SerializeField]
  protected float damage;
  [SerializeField]
  protected float szybkostrzelnosc;
  [SerializeField]
  protected float range;
  [SerializeField]
  protected float impactforce;
  [SerializeField]
  protected float nextTimeToFire;
  [SerializeField]
  protected float reloadTime;
  [SerializeField]
  public int maxAmmo;
  [SerializeField]
  public int currentAmmo;
  [SerializeField]
  protected bool isReloading;
  [SerializeField]
  public Text CurrentAmmoText;
  [SerializeField]
  public Text MaxAmmoText;
  [SerializeField]
  public Text skocznik;
  public AudioClip Dzwiek;
  public AudioClip Przeladowanie;
  private int PktZaTrafieni = 10;
  public Wynik pkt;

  public virtual void Start()
  {
    this.currentAmmo = this.maxAmmo;
    this.Dzwiek = Graczmenager.instance.dzwiek;
    this.Przeladowanie = Graczmenager.instance.Przeladowanie;
  }

  public virtual void Update()
  {
    if (this.isReloading)
      return;
    if (this.currentAmmo <= 0)
    {
      this.StartCoroutine(this.Reload());
      AudioManager.Instance.Play(this.Przeladowanie);
    }
    else if (this.currentAmmo < this.maxAmmo && Input.GetKey("r"))
    {
      this.StartCoroutine(this.Reload());
      AudioManager.Instance.Play(this.Przeladowanie);
    }
    if (!Input.GetButton("Fire1") || (double) Time.time < (double) this.nextTimeToFire)
      return;
    this.nextTimeToFire = Time.time + 1f / this.szybkostrzelnosc;
    this.Shoot();
    AudioManager.Instance.Play(this.Dzwiek);
  }

  public virtual void Shoot()
  {
    this.Wystrzal.Play();
    --this.currentAmmo;
    RaycastHit hitInfo;
    if (!Physics.Raycast(this.fpscamera.transform.position, this.fpscamera.transform.forward, out hitInfo, this.range))
      return;
    Object.Destroy((Object) Object.Instantiate<GameObject>(this.impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)), 2f);
    ai component1 = hitInfo.transform.GetComponent<ai>();
    AILucznik component2 = hitInfo.transform.GetComponent<AILucznik>();
    Debug.Log((object) hitInfo.transform.name);
    if ((Object) component1 != (Object) null)
    {
      component1.GetComponent<stats>().takeDamage(this.damage);
      this.pkt.ZliczPKT(this.PktZaTrafieni);
      component1.lookRadius = 60f;
    }
    if (!((Object) component2 != (Object) null))
      return;
    component2.GetComponent<StatsLucznik>().takeDamage(this.damage);
    this.pkt.ZliczPKT(this.PktZaTrafieni);
  }

  public virtual void OnEnable()
  {
    this.isReloading = false;
    this.anim.SetBool("Przeladowanie", false);
  }

  public IEnumerator Reload()
  {
    this.isReloading = true;
    this.anim.SetBool("Przeladowanie", true);
    yield return (object) new WaitForSeconds(this.reloadTime);
    this.anim.SetBool("Przeladowanie", false);
    this.currentAmmo = this.maxAmmo;
    this.isReloading = false;
  }

  public virtual void ZliczAmunicje()
  {
    this.CurrentAmmoText.text = this.currentAmmo.ToString();
    this.MaxAmmoText.text = this.maxAmmo.ToString();
  }
}
