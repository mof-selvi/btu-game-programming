### DNA Inspired Veri Yapıları

*   Çevresel adaptasyona bağlı zeka selection (seçilim), reproduction (üreme), mutation (mutasyon) kavramlarını gerçekleştirerek benzetilebilir.
*   Genler atadan çocuklara aktarılır.
*   Fitness (uygunluk) fonksiyonuna göre en iyiler seçilir (diğer bir ifadeyle kötüler elenir).
*   En iyiler genlerini bir sonraki nesle aktarır.
*   **Örnek:** Kırmızı, sarı ve yeşil rengi daha iyi fark edemeyenlerin elenmesi, renkli sineklerin ölmesi vb.

---

### Eleme Örneği

*   Bir grup nesne (sinek) oluşturun. Bunların renkleri `red=0.3`, `green=0.8`, `blue=0.5` gibi rastgele değerlere sahip olsun.
*   Avcılar nesneleri en parlak önce olmak üzere yesin (teker teker nesnelerin üzerini tıklayarak temsil edilebilir).
*   Her bireyin ne kadar uzun süre hayatta kaldığını kaydedin.
*   Bir sonraki nesil oluşturulurken daha uzun süre hayatta kalanların genetik özellikleri (r,g,b) dikkate alınsın.
*   Yeni nesil oluşturulurken anne ve babanın genetik özellikleri arasından rastgele seçim yapılsın `r_a`, `g_b`, `b_b` (anneden r, babadan g, b gibi).
*   Bu işlem birkaç nesil tekrar etsin.
*   Yeni nesillerde gittikçe daha fazla mat nesnenin oluşmaya başladığına dikkat edin.

---

### Genetik Algoritmayla Kamuflaj Eğitimi (Kurulum)

*   Yeni bir Unity 2D proje oluşturun.
*   Proje dosyasını ( [CamoGATraining](CamoGATraining.unitypackage) ) import edin.
*   Varsayılan sahneyi silerek **camo** sahnesini Scenes klasörüne taşıyın.
*   Assets altında **Prefabs** klasörü oluşturun ve Person’u bu klasöre taşıyın.
*   Assets altında **Scripts** klasörü oluşturun. Bu klasör altında yeni bir C# script oluşturun: `DNA_sc` ve script’i nesneye ekleyin.
*   Script’i açın ve renk kodları geni için üç değişken tanımlayın:
    ```csharp
    public float r;
    public float g;
    public float b;
    ```
*   Nesnenin tıklanıp tıklanmadığını (ölüm) takip etmek için bir değişken tanımlayın:
    ```csharp
    bool isDead = false;
    ```
*   Yaşam süresini takip etmek için değişken tanımlayın:
    ```csharp
    public float timeToDie = 0;
    ```
*   Nesne öldükten sonra ekrandan kaybolmasını sağlamak (SpriteRenderer) ve ekranda görünmezken diğer nesnelerle çarpışmasını engellemek (Collider2D) için değişken tanımlayın:
    ```csharp
    SpriteRenderer sRenderer;
    Collider2D sCollider;
    ```
*   Tanımladığımız son iki değişkeni Start fonksiyonu içinde ilklendirelim (initialize):
    ```csharp
    sRenderer = GetComponent<SpriteRenderer>();
    sCollider = GetComponent<Collider2D>();
    ```
    *(Not: GetComponent fonksiyonu ile nesne üzerinde bulunan bileşenleri (component) getirebiliriz. Bir nesne üzerinde hangi bileşenler var kontrol etmek için nesneyi seçtikten sonra Inspector penceresi incelenebilir.)*

*   Birazdan kodla yapılacak işlemi önce elle test etmek için Person prefab’ini sahneye sürükle bırak yapalım. Nesne seçiliyken Inspector penceresinde SpriteRenderer bileşenini bulun ve bu bileşenin başlığının en solunda yer alan kontrol kutusundaki (checkbox) işareti kaldırın. Nesnenin sahneden kaybolduğuna dikkat edin.

---

### Tıklama ve Ölüm Kontrolü

*   Oyunda avcının avını öldürmesini basitleştirerek farenin sol tıklanması ile temsil edeceğiz.
*   Unity’de farenin tıklanmasını tespit eden hazır bir fonksiyon mevcut: `OnMouseDown()`
*   Bu fonksiyonu tanımlayarak içini dolduralım:
    ```csharp
    void OnMouseDown()
    {
        isDead = true;
        timeToDie = PopulationManager.elapsed;
        Debug.Log("Dead At: " + timeToDie);
        sRenderer.enabled = false;
        sCollider.enabled = false;
    }
    ```

---

### PopulationManager Nesnesini Tanımlama

*   **PopulationManager** nesillerin oluşmasını kontrol edecek.
*   Farklı nesilleri temsil etmek için döngüler (cycle) tanımlayacağız.
*   Her döngü başladığında sayaç başlayacak ve av öldüğünde sayacın ne kadar ilerlemiş olduğunu kontrol edeceğiz.
*   PopulationManager için boş bir oyun nesnesi tanımlayalım:
    *   Hierarchy penceresinde sağ tıkla ve **Create Empty** seç.
    *   İsmini **PopulationManager** olarak değiştir.
*   Kodu yazmak için yeni bir script tanımlayın: `PopulationManager_sc`
    *   Assets – Scripts altında oluşturun.
    *   Script’i PopulationManager nesnesi ile ilişkilendirin.

---

### PopulationManager_sc Kod Düzenlemesi

*   Person prefab’ine referans için bir değişken tanımlayalım. Unity ekranından değer ataması yapabilmek için public tanımlıyoruz:
    ```csharp
    public GameObject personPrefab;
    ```
    *(Unity ekranında Hierarchy penceresinde PopulationManager nesnesini seçin. Inspector penceresinde Person Prefab üzerine Proje penceresinden Assets – Prefabs altından Person prefab’ini sürükleyip bırakın.)*

*   Populasyon büyüklüğü için değişken tanımlayın:
    ```csharp
    public int populationSize = 10;
    ```
*   Oluşturulan tüm popülasyon için array tanımlayın:
    ```csharp
    List<GameObject> population = new List<GameObject>();
    ```
*   Her yeni döngü içinde zamanın ne kadar ilerlediğini saklamak için:
    ```csharp
    public static float elapsed = 0;
    ```

*   **PopulationManager** popülasyon büyüklüğü kadar rastgele Person oluşturacak. Start() fonksiyonu içine aşağıdaki kodu ekleyin:
    ```csharp
    for (int i=0; i<populationSize; i++)
    {
        Vector3 pos = new Vector3(Random.Range(-9.5f,9.5f), Random.Range(-3.4f,5.4f), 0);
        GameObject o = Instantiate(personPrefab, pos, Quaternion.identity);
        o.GetComponent<DNA_sc>().r = Random.Range(0.0f, 1.0f);
        o.GetComponent<DNA_sc>().g = Random.Range(0.0f, 1.0f);
        o.GetComponent<DNA_sc>().b = Random.Range(0.0f, 1.0f);
        population.Add(o);
    }
    ```
    *(Rastgele konum aralıklarını belirlerken Sahne penceresinde Person prefab’i ile test yapın. Ekran çözünürlüğüne bağlı olarak bu değerler değişebilir. Renk aralıkları 0-1 arasında belirlenecek.)*

---

### Nesneleri Renklendirme

*   Kodu kaydedip oyunu çalıştırın. Sahnede 10 nesne oluşacak ancak hepsi beyaz renkli. Düzeltmek için **DNA_sc** scriptini açın.
*   Start() fonksiyonu içinde Renderer kullanarak rengi atayın:
    ```csharp
    sRenderer.color = new Color(r, g, b);
    ```
*   Kaydedin ve tekrar çalıştırın. Nesneler rastgele atanmış farklı renk kodları ile oluşturuldu.
*   Nesnelere fare ile tıkladığımızda kaybolduklarını test edebilirsiniz.

---

### Jenerasyon Kontrolü ve GUI

*   Her döngünün ne kadar süreceğini tanımlayalım:
    ```csharp
    int trialTime = 10; // 10 saniye içinde tamamını avlayacağımızı varsayıyoruz.
    int generation = 1; // Hangi jenerasyonda olduğumuzu saklar.
    ```
*   Ekranda jenerasyon no ve geçen sürenin gösterimi için:
    ```csharp
    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 20;
        guiStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(10,10,100,20), "Generation: " + generation, guiStyle);
        GUI.Label(new Rect(10,30,100,20), "Time: " + (int) elapsed, guiStyle);
    }
    ```
*   Her jenerasyon 10 saniye sürecek ve ardından yeni jenerasyon oluşturulacak. Update() fonksiyonu içinde zamanı ilerleterek jenerasyon değişikliğini sağlayalım:
    ```csharp
    elapsed += Time.deltaTime;
    if (elapsed > trialTime)
    {
        BreedNewPopulation();
        elapsed = 0;
    }
    ```

---

### Yeni Nesil Oluşturma (Breeding)

*   Sıralama işlemlerinde kullanmak için Linq kütüphanesini ekleyin:
    ```csharp
    using System.Linq;
    ```
*   Yeni nesli oluşturmak için **BreedNewPopulation** fonksiyonunu tanımlayalım. Bu fonksiyon:
    *   Popülasyondakileri (ilk ölen ilk sırada olacak şekilde) yaşam sürelerine göre sıralar.
    *   Popülasyonun daha uzun yaşayanlarını kapsayacak şekilde yarısını dikkate alarak (%75 vb. de olabilirdi) çiftleştirme yapar.
    *   Bu sayede daha uzun süre hayatta kalanlar kendi genlerini sonraki nesle aktarır.
    *   Her yeni nesilde genlerini aktaranların özellikleri daha baskın hale gelir.

*   **BreedNewPopulation Kodu:**
    ```csharp
    void BreedNewPopulation()
    {
        List<GameObject> newPopulation = new List<GameObject>();
        // Linq ile sıralama: az yaşayandan çok yaşayana doğru
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<DNA_sc>().timeToDie).ToList();
        
        population.Clear();
        
        // Listenin ikinci yarısını (uzun yaşayanları) çiftleştir
        for (int i = (int)(sortedList.Count/2.0f)-1; i<sortedList.Count-1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i+1]));
            population.Add(Breed(sortedList[i+1], sortedList[i]));
        }
        
        // Eski nesli yok et
        for (int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        generation++;
    }
    ```

*   **Breed Kodu:**
    ```csharp
    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 pos = new Vector3(Random.Range(-9.5f,9.5f), Random.Range(-3.4f,5.4f), 0);
        GameObject offspring = Instantiate(personPrefab, pos, Quaternion.identity);
        
        DNA_sc dna1 = parent1.GetComponent<DNA_sc>();
        DNA_sc dna2 = parent2.GetComponent<DNA_sc>();
        
        // Gen aktarımı (%50 şans ile anneden veya babadan)
        offspring.GetComponent<DNA_sc>().r = Random.Range(0, 10) < 5 ? dna1.r : dna2.r;
        offspring.GetComponent<DNA_sc>().g = Random.Range(0, 10) < 5 ? dna1.g : dna2.g;
        offspring.GetComponent<DNA_sc>().b = Random.Range(0, 10) < 5 ? dna1.b : dna2.b;
        
        return offspring;
    }
    ```

---

### Kamuflaj Etkisi ve Oyunlaştırma

*   Kodu kaydedin ve oyunu çalıştırın.
*   Eğer parlak renkli olanları önce seçerseniz mat renkli olanlar daha uzun yaşar ve sonraki nesle kendi genlerini aktarırlar.
*   Bu şekilde tekrar eden birkaç nesil sonra popülasyonun renginin gittikçe matlaştığına dikkat edin.
*   Bu işlemin tersi de uygulanabilir. Eğer mat renklileri önce seçerseniz popülasyondaki parlak renklilerin oranı gittikçe artacaktır.
*   **Favori Renk Oyunu:** Kodda ufak bir değişiklikle (örneğin `OrderBy` yerine `OrderByDescending` kullanarak) en sevdiğiniz rengi önce seçtiğiniz bir oyun kurgulayabilirsiniz. Her yeni nesilde, sevdiğiniz renklerin popülasyonda daha baskın hale geldiğine dikkat edin.

---

### Kalıtıma Mutasyon Eklemek

*   Mutasyon ile kalıtımdan beklenenin dışına çıkılabilir. Mutasyon, yeni genin oluştuğu noktada (Breed fonksiyonu) etki edecek. Mutasyon ihtimalini etkinleştirmek için kodda değişiklik yapalım:

    ```csharp
    if (Random.Range(0,10) < 5) 
    {
        // Normal kalıtım
        offspring.GetComponent<DNA_sc>().r = Random.Range(0, 10) < 5 ? dna1.r : dna2.r;
        offspring.GetComponent<DNA_sc>().g = Random.Range(0, 10) < 5 ? dna1.g : dna2.g;
        offspring.GetComponent<DNA_sc>().b = Random.Range(0, 10) < 5 ? dna1.b : dna2.b;
    } 
    else 
    { 
        // %50 mutasyon ihtimali (Genelde mutasyon ihtimali çok daha düşüktür)
        offspring.GetComponent<DNA_sc>().r = Random.Range(0.0f, 1.0f);
        offspring.GetComponent<DNA_sc>().g = Random.Range(0.0f, 1.0f);
        offspring.GetComponent<DNA_sc>().b = Random.Range(0.0f, 1.0f);
    }
    ```

---

### Kendimizi Test Edelim: Boyut Bilgisi Ekleme

*   Genetik koda nesnelerin boyut bilgisini de ekleyelim.

**Çözüm Adımları:**
1.  **DNA_sc** dosyasında boyut için değişken tanımlayalım:
    ```csharp
    public float s;
    ```
2.  **DNA_sc** dosyasındaki `Start()` içinde boyutu tanımlayalım:
    ```csharp
    this.transform.localScale = new Vector3(s, s, s);
    ```
3.  **PopulationManager** dosyasında renk ataması yapılan yerde boyut ataması da yapalım:
    ```csharp
    o.GetComponent<DNA_sc>().s = Random.Range(0.1f, 0.3f);
    ```
4.  **Breed** fonksiyonunda boyutu dikkate alalım:
    ```csharp
    if (...) {
        // Normal kalıtım
        offspring.GetComponent<DNA_sc>().s = Random.Range(0, 10) < 5 ? dna1.s : dna2.s;
    } else {
        // Mutasyon
        offspring.GetComponent<DNA_sc>().s = Random.Range(0.1f, 0.3f);
    }
    ```
