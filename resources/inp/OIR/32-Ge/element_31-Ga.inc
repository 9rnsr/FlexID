[{Ga}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood
  acc   C-bone-S              C-bone-S
  acc   T-bone-S              T-bone-S
  acc   RedMarrow             R-marrow
  acc   Liver                 Liver
  acc   Kidneys               Kidneys
  acc   Pancreas              Pancreas
  acc   Spleen                Spleen
  acc   Muscle                Muscle
  acc   ST0                   Other
  acc   ST1                   Other


[{Ga}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.151 p.164 Table 17.3
  Blood                   RC-con                    0.15
  Blood                   Liver                     0.3
  Blood                   Kidneys                   0.4
  Blood                   Spleen                    0.05
  Blood                   T-bone-S                  0.5
  Blood                   C-bone-S                  0.5
  Blood                   RedMarrow                 0.25
  Blood                   Muscle                    0.2
  Blood                   Pancreas                  0.005
  Blood                   ST0                       2.145
  Blood                   ST1                       0.5
  Liver                   Blood                     0.139
  Kidneys                 UB-con                    1.39
  Spleen                  Blood                     0.139
  T-bone-S                Blood                     0.347
  C-bone-S                Blood                     0.347
  RedMarrow               Blood                     0.347
  Muscle                  Blood                     0.139
  Pancreas                Blood                     0.139
  ST0                     Blood                     1.39
  ST1                     Blood                     0.0019

# ICRP Publ.151 p.173 Para.343
# > (343) Progeny of germanium addressed in this publication are radioisotopes of
# > gallium and arsenic. The models for gallium and arsenic as germanium progeny are
# > expansions of the characteristic models for these elements with added compartments
# > and associated transfer coefficients needed to solve the linked biokinetic
# > models for chains headed by germanium (see Annex B). If produced in a compartment
# > not explicitly named in the progeny’s model, the progeny is assumed to
# > transfer to the central blood compartment of its characteristic biokinetic model,
# > and to follow that model thereafter. The assigned transfer rate to the central blood
# > compartment is the rate of bone turnover for the indicated bone type if the progeny
# > is produced in a bone volume compartment, and at the following elementspecific
# > rate if produced in any other ambiguous compartment: gallium, 1.39 d⁻¹;
# > and arsenic, 0.6 d⁻¹.
# > (343) 本書で扱うゲルマニウムの娘核種は、ガリウムおよびヒ素の放射性同位体です。
# > ゲルマニウムの娘核種としてのガリウムおよびヒ素に関するモデルは、各元素固有のモデルを
# > 拡張したものです。これには、ゲルマニウムを起点とする崩壊系列の連鎖的な生物動態モデルを
# > 解くために必要な、コンパートメントの追加およびそれに関連する移行係数が含まれています
# > （付録Bを参照）。娘核種が、そのモデル内で明示的に指定されていないコンパートメントで
# > 生成された場合、その娘核種は、当該元素固有の生物動態モデルにおける中心血液コンパートメントへ
# > 移行し、その後はそのモデルに従うものと仮定されます。中心血液コンパートメントへの移行速度は、
# > 娘核種が骨体積コンパートメントで生成された場合には、当該骨種における骨代謝回転速度となります。
# > それ以外の、明確に定義されていないコンパートメントで生成された場合には、元素ごとに
# > 以下の移行速度を適用します：ガリウムについては1.39 d⁻¹、ヒ素については0.6 d⁻¹です。

  {Ge}/Blood              Blood                     ---

  {Ge}/C-bone-S           C-bone-S                  ---
  {Ge}/T-bone-S           T-bone-S                  ---
  {Ge}/C-bone-V           Blood                 $( 3% / 365)
  {Ge}/T-bone-V           Blood                 $(18% / 365)

  {Ge}/Liver              Liver                     ---
  {Ge}/Kidneys            Kidneys                   ---

  {Ge}/Other              Blood                     1.39
