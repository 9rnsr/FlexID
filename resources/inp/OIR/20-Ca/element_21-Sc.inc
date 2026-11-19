[{Sc}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood
  acc   Blood2                Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   C-bone-V              C-bone-V
  acc   T-bone-V              T-bone-V
  acc   C-marrow              C-marrow
  acc   T-marrow              T-marrow
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys               Kidneys
  acc   Spleen                Spleen
  acc   ST0                   Other
  acc   ST1                   Other


[{Sc}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.134 p.125 Para.247
# > (247) It is assumed in this publication that 47Sc produced by decay of 47Ca in soft
# > tissues is removed to blood with a biological half-time of 3 d, and then follows the
# > characteristic model for scandium, i.e. behaves as if injected into blood as a parent
# > radionuclide. The removal half-time of 3 d is the shortest removal half-time of scandium
# > from tissues in the characteristic model for scandium used here. 47Sc produced
# > in a bone compartment of the calcium model is assumed to follow the characteristic
# > model for scandium described below.
# > (247) 本書では、軟組織において47Caの崩壊により生成された47Scは、生物学的半減期3日で
# > 血液中に移行し、その後、スカンジウムに関する特性モデルに従う、すなわち親放射性核種として
# > 血液中に注入された場合と同様の挙動を示すと仮定されています。この3日という移行半減期は、
# > ここで用いるスカンジウムの特性モデルにおける組織からのスカンジウムの移行半減期のうち、
# > 最短のものとなります。カルシウムモデルの骨コンパートメント内で生成された47Scは、
# > 以下に述べるスカンジウムの特性モデルに従うものと仮定されます。

  {Ca}/Blood              Blood1                    ---

  {Ca}/C-bone-S           C-bone-S                  ---
  {Ca}/T-bone-S           T-bone-S                  ---
  {Ca}/Exch-C-bone-V      C-bone-V                  ---
  {Ca}/Exch-T-bone-V      T-bone-V                  ---
  {Ca}/Noch-C-bone-V      C-bone-V                  ---
  {Ca}/Noch-T-bone-V      T-bone-V                  ---

  {Ca}/ST0                Blood1                $(0.693 / 3)
  {Ca}/ST1                Blood1                $(0.693 / 3)
  {Ca}/ST2                Blood1                $(0.693 / 3)

# ICRP Publ.134 p.125 Para.248
# > (248) The structure of the characteristic model for scandium is a modification of
# > the generic model structure applied in the OIR series to bone-surface-seeking radionuclides.
# > Scandium is treated as a bone-surface seeker based on analogy with its
# > chemical analogue yttrium. The spleen is added to the generic model structure
# > because human and animal data indicate that it is an important repository for scandium.
# > The generic structure is also modified with regard to routes of transfer to and
# > from the marrow compartments, based on indications from animal studies of relatively
# > high transfer of scandium from plasma to marrow (Rosoff et al., 1963, 1965;
# > Hara and Freed, 1973; Byrd et al., 1975; Lachine et al., 1976).
# > (248) スカンジウムに関する特性モデルの構造は、OIRシリーズにおいて骨表面親和性放射性核種に
# > 適用された汎用モデル構造を改変したものです。スカンジウムは、化学的類似体であるイットリウムとの
# > 類推に基づき、骨表面親和性核種として扱われます。ヒトおよび動物のデータから、脾臓がスカンジウムの
# > 重要な貯蔵部位であることが示唆されているため、汎用モデル構造に脾臓が追加されています。また、
# > 血漿から骨髄へのスカンジウムの移行が比較的高いことを示す動物実験の結果（Rosoff et al., 1963, 1965;
# > Hara and Freed, 1973; Byrd et al., 1975; Lachine et al., 1976）に基づいて、
# > 骨髄コンパートメントへの移行およびそこからの移行経路についても、汎用構造が改変されています。

# ICRP Publ.134 p.126 Para.249
# > (249) Transfer coefficients in the characteristic model for scandium are set for
# > consistency with the following data: (1) for human subjects, measurements of
# > blood clearance over 3 d, urinary and faecal excretion rates over 15 d, whole-body
# > retention over 1.5 y, and activity concentrations in autopsy tissues of subjects dying
# > 5–7 months after injection (Rosoff et al., 1965); and (2) measurements of the timedependent
# > systemic distribution of activity in rats, mice, and rabbits (Durbin, 1960;
# > Rosoff et al., 1963; Basse-Cathalinat et al., 1968; Hara and Freed, 1973; Byrd et al.,
# > 1975; Lachine et al., 1976).
# > (249) スカンジウムに関する特性モデルにおける移行係数は、以下のデータと整合するように
# > 設定されています。(1) ヒト被験者における、3日間にわたる血中クリアランス、15日間にわたる
# > 尿および糞への排泄率、1.5年間にわたる全身保持、ならびに投与後5～7ヶ月で死亡した被験者の
# > 剖検組織中の放射能濃度（Rosoff et al., 1965）の測定値。(2) ラット、マウス、およびウサギに
# > おける放射能の経時的な全身分布の測定値（Durbin, 1960; Rosoff et al., 1963;
# > Basse-Cathalinat et al., 1968; Hara and Freed, 1973; Byrd et al., 1975; Lachine et al., 1976）。

# ICRP Publ.134 p.126 Para.250-251
# > (250) Blood is divided into two compartments: Blood 1 and Blood 2. These represent
# > two components of retention as indicated by data for intravenously injected
# > 46Sc NTA in human subjects (Rosoff et al., 1965). Blood 1 is a central compartment
# > that exchanges activity with Blood 2 and several tissue compartments. 47Sc migrating
# > to blood from sites of production is assigned to Blood 1. Blood 2 represents scandium
# > that is firmly bound to plasma proteins.
# > (250) 血液は、「血液1」および「血液2」という2つのコンパートメントに区分されます。
# > これらは、ヒト被験者への46Sc-NTAの静脈内投与データ（Rosoff et al., 1965）によって
# > 示される保持の2つの成分を表しています。「血液1」は中心コンパートメントであり、
# > 「血液2」やいくつかの組織コンパートメントとの間で放射能の交換を行います。生成部位から
# > 血液へ移行する47Scは、「血液1」に割り当てられます。「血液2」は、血漿タンパク質に強固に結合した
# > スカンジウムを表します。

# ICRP Publ.134 p.126 Para.251
# > (251) The total outflow rate from Blood 1 is 3 d1. Blood 2 receives 15% of
# > outflow from Blood 1, and loses scandium back to Blood 1 with a half-time of
# > 1.5 d. This half-time is taken from the model applied in the OIR series to the chemically
# > similar element yttrium.
# > (251) 血液1（Blood 1）からの総流出率は3 d⁻¹です。血液2（Blood 2）は血液1からの流出量の
# > 15%を受け取り、半減期1.5日でスカンジウムを血液1へ戻します。この半減期は、OIRシリーズにおいて
# > 化学的性質の類似した元素であるイットリウムに適用されたモデルから採用されています。

# ICRP Publ.134 p.126 Para.252
# > (252) The liver is divided into two compartments: Liver 1 and Liver 2. Liver 1
# > receives 20% of outflow from Blood 1. Activity is removed from Liver 1 with a halftime
# > of 3 d, with 50% moving to Blood 1, 25% to Liver 2, and 25% to the small
# > intestine contents (representing biliary secretion). Faecal excretion of scandium is
# > assumed to arise solely from transfer of scandium from Liver 1 to the small intestine
# > contents based on data of Rosoff et al. (1965) for a human subject. Almost all of the
# > scandium secreted into the small intestine is lost in faeces because of the low rate of
# > absorption of scandium from the small intestine to blood. Activity transfers from
# > Liver 2 to Blood 1 with a half-time of 100 d.
# > (252) 肝臓は、肝臓1（Liver 1）と肝臓2（Liver 2）の2つのコンパートメントに分けられます。
# > 肝臓1は血液1からの流出量の20%を受け取ります。放射能は半減期3日で肝臓1から消失し、
# > その50%は血液1へ、25%は肝臓2へ、25%は小腸内容物（胆汁分泌を表す）へ移行します。
# > スカンジウムの糞中排泄は、ヒト被験者を対象としたRosoffら（1965）のデータに基づき、
# > 肝臓1から小腸内容物へのスカンジウムの移行のみに由来すると仮定されます。小腸から血液への
# > スカンジウムの吸収率が低いため、小腸に分泌されたスカンジウムはほぼ全量が糞便とともに
# > 排出されます。放射能は半減期100日で肝臓2から血液1へ移行します。

# ICRP Publ.134 p.126 Para.253
# > (253) The kidneys are represented as a single compartment that exchanges activity
# > with Blood 1. This compartment receives 3% of outflow from Blood 1, and loses
# > scandium to Blood 1 with a half-time of 20 d. Urinary excretion of scandium is
# > represented as a direct transfer from Blood 1 to urinary bladder contents, without
# > intermediate retention in the kidneys. Urinary bladder contents receives 1.8% of
# > outflow from Blood 1.
# > (253) 腎臓は、血液1と放射能をやり取りする単一のコンパートメントとして表されます。
# > このコンパートメントは血液1からの流出量の3%を受け取り、半減期20日でスカンジウムを
# > 血液1へ放出します。スカンジウムの尿中排泄は、腎臓内での一時的な滞留を経ることなく、
# > 血液1から膀胱内容物（urinary bladder contents）へ直接移行するものとして表されます。
# > 膀胱内容物は血液1からの流出量の1.8%を受け取ります。

# ICRP Publ.134 p.126 Para.254
# > (254) Trabecular and cortical marrow each receive 5% of outflow from Blood 1.
# > Activity is removed from the marrow compartments to Blood 1 with a half-time of 100 d.
# > (254) 海綿骨髄（trabecular marrow）および皮質骨髄（cortical marrow）は、それぞれ
# > 血液1からの流出量の5%を受け取ります。放射能は半減期100日で骨髄コンパートメントから
# > 血液1へ移行します。

# ICRP Publ.134 p.126 Para.255
# > (255) The spleen receives 2% of outflow from Blood 1. The removal half-time
# > from spleen to Blood 1 is 1 y.
# > (255) 脾臓は「血液1」からの流出量の2%を受け取ります。脾臓から「血液1」への
# > 除去半減期は1年です。

# ICRP Publ.134 p.126 Para.256
# > (256) Other soft tissues are divided into two compartments representing relatively
# > fast (t½=3 d) and relatively slow (t½=100 d) return of scandium to Blood 1. These
# > compartments receive 20% and 18.2% of outflow from Blood 1, respectively.
# > The deposition fraction in the latter compartment is the balance of outflow from
# > Blood 1 after all other deposition fractions in the model were assigned.
# > (256) その他の軟部組織は、スカンジウムが「血液1」へ戻る速度が比較的速い区画（半減期3日）と、
# > 比較的遅い区画（半減期100日）の2つに分けられます。これらの区画は、「血液1」からの流出量の
# > それぞれ20%と18.2%を受け取ります。後者の区画における沈着割合は、モデル内の他のすべての
# > 沈着割合を割り当てた後に残る、「血液1」からの流出量の残余分です。

# ICRP Publ.134 p.126 Para.257
# > (257) Bone surface receives 10% of outflow from Blood 1. The deposition on bone
# > surface is divided equally between trabecular and cortical surfaces. The fate of
# > scandium deposited on bone surfaces is described by the generic model for bonesurface
# > seekers, except that scandium removed biologically from bone is assumed to
# > return to blood rather than channelled through bone marrow. Thus, scandium is
# > removed from cortical or trabecular bone surfaces at a rate proportional to (1.5
# > times) the turnover rate of that bone type. The assumed bone turnover rates are
# > 3% y1 for cortical bone and 18% y1 for trabecular bone. One-third of activity
# > removed from bone surfaces is buried in bone volume, and two-thirds transfers to
# > Blood 1. Activity is removed from cortical or trabecular bone volume to Blood 1 at
# > the rate of turnover of that bone type.
# > (257) 骨表面は「血液1」からの流出量の10%を受け取ります。骨表面への沈着量は、海綿骨表面と
# > 皮質骨表面に均等に分配されます。骨表面に沈着したスカンジウムの挙動は、骨表面親和性核種に
# > 関する一般モデルに従って記述されますが、骨から生物学的に除去されたスカンジウムは、
# > 骨髄を経由するのではなく血液に戻ると仮定される点が異なります。したがって、スカンジウムは、
# > 皮質骨または海綿骨の表面から、その骨型のターンオーバー率に比例した速度（1.5倍）で
# > 除去されます。仮定される骨ターンオーバー率は、皮質骨で年3%、海綿骨で年18%です。
# > 骨表面から除去された放射能の3分の1は骨体積内に埋没し、3分の2は「血液1」へ移行します。
# > 皮質骨または海綿骨の体積から「血液1」へ除去される放射能は、その骨型の代謝率に従います。

  Blood1                  Blood2                $(3 * 15%      )
  Blood1                  Liver1                $(3 * 20%      )
  Blood1                  Kidneys               $(3 *  3%      )
  Blood1                  UB-con                $(3 *  1.8%    )
  Blood1                  T-marrow              $(3 *  5%      )
  Blood1                  C-marrow              $(3 *  5%      )
  Blood1                  Spleen                $(3 *  2%      )
  Blood1                  ST0                   $(3 * 20%      )
  Blood1                  ST1                   $(3 * 18.2%    )
  Blood1                  T-bone-S              $(3 * 10% * 0.5)
  Blood1                  C-bone-S              $(3 * 10% * 0.5)
  Blood2                  Blood1                $(0.693 / 1.5      )
  Liver1                  SI-conRe              $(0.693 /   3 * 25%)
  Liver1                  Blood1                $(0.693 /   3 * 50%)
  Liver1                  Liver2                $(0.693 /   3 * 25%)
  Liver2                  Blood1                $(0.693 / 100      )
  Kidneys                 Blood1                $(0.693 /  20      )
  T-marrow                Blood1                $(0.693 / 100      )
  C-marrow                Blood1                $(0.693 / 100      )
  Spleen                  Blood1                $(0.693 / 365      )
  ST0                     Blood1                $(0.693 /   3      )
  ST1                     Blood1                $(0.693 / 100      )
  T-bone-S                Blood1                $(0.18 / 365 * 1.5 * 2/3)
  T-bone-S                T-bone-V              $(0.18 / 365 * 1.5 * 1/3)
  T-bone-V                Blood1                $(0.18 / 365            )
  C-bone-S                Blood1                $(0.03 / 365 * 1.5 * 2/3)
  C-bone-S                C-bone-V              $(0.03 / 365 * 1.5 * 1/3)
  C-bone-V                Blood1                $(0.03 / 365            )
