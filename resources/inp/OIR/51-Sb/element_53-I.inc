# ICRP Publ.137 p.62 Para.135 / アンチモンの子孫核種としてのヨウ素
# > (135) The characteristic model for iodine is defined in Fig. 5.2 and Table 5.4 of this
# > publication. Two features of that model were simplified for treatment of iodine
# > produced in vivo after intake of antimony. ...
# > (135) ヨウ素の特性モデルは、本書の図5.2および表5.4に定義されています。このモデルの2つの特徴は、
# > アンチモン摂取後に体内で生成されるヨウ素の処理のために簡素化されました。 ...

[{I}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood1                Blood         # inorganic iodine in blood
  acc   Blood2                Blood         #   organic iodine in blood
  acc   S-glands              S-glands
  acc   OralcavityRe          O-cavity
  acc   Oesophagus-sRe        Oesophag-s
  acc   St-conRe              St-cont
  acc   St-wall               St-wall
  acc   Thyroid1              Thyroid
  acc   Thyroid2              Thyroid
  acc   Liver1                Liver
  acc   Liver2                Liver
  acc   Kidneys1              Kidneys
  acc   Kidneys2              Kidneys
$if parent
  acc   Other1                Other
  acc   Other2                Other
  acc   Other3                Other
  acc   Other4                Other
$endif
  acc   Other1                Other         # inorganic
  acc   Other2                Other         #   organic


[{I}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

# ICRP Publ.130 p.76 Table 3.4
# ICRP Publ.130 p.77 Para.143
# ICRP Publ.137 p.106 Fig.5.2
  OralcavityRe            Oesophagus-sRe          720
  Oesophagus-sRe          St-conRe               2160
  St-conRe                SI-conRe                 20.57

# ICRP Publ.137 p.108 Table 5.4
  Blood1                  Thyroid1                  7.26
  Blood1                  UB-con                   11.84
  Blood1                  S-glands                  5.16
  Blood1                  St-wall                   8.60
$if parent
  Blood1                  Other1                  600
$endif
  Blood1                  Kidneys1                 25
  Blood1                  Liver1                   15
  S-glands                OralcavityRe             50
  St-wall                 St-conRe                 50
  Thyroid1                Thyroid2                 95
  Thyroid1                Blood1                   36
  Thyroid2                Blood2                    0.0077
  Thyroid2                Blood1                    0
$if parent
  Other1                  Blood1                  330
  Other1                  Other2                   35
  Other2                  Other1                   56
$endif
  Kidneys1                Blood1                  100
  Liver1                  Blood1                  100
$if parent
  Blood2                  Other3                   15
  Other3                  Blood2                   21
  Other3                  Other4                    1.2
  Other4                  Other3                    0.62
  Other4                  Blood1                    0.14
$endif
  Blood2                  Kidneys2                  3.6
  Kidneys2                Blood2                   21
  Kidneys2                Blood1                    0.14
  Blood2                  Liver2                   21
  Liver2                  Blood2                   21
  Liver2                  Blood1                    0.14
  Liver2                  RC-con                    0.08

$if progeny

# ICRP Publ.137 p.62 Para.135
# > (135) ... Firstly, the two-compartment submodel
# > of inorganic iodide in ‘other tissue’ was replaced by a single compartment called
# > ‘Other 1’ that exchanges inorganic iodide with a blood compartment called ‘Blood 1’.
# > The following transfer coefficients were assigned: Blood 1 to Other 1, 600 d⁻¹; and
# > Other 1 to Blood 1, 200 d⁻¹. ...
# > (135) ... 第一に、「その他の組織」中の無機ヨウ化物の2つのコンパートメントサブモデルは、
# > 「血液1」と呼ばれる血液コンパートメントと無機ヨウ化物を交換する「その他1」と呼ばれる
# > 単一のコンパートメントに置き換えられました。以下の移動係数が割り当てられました：
# >   血液1からその他1, 600 d⁻¹;
# >   その他1から血液1, 200 d⁻¹。 ...

# (Other1 & Other2) -> Other1, and
  Blood1                  Other1                  600
  Other1                  Blood1                  200

# ICRP Publ.137 p.62 Para.135
# > (135) ... Secondly, the two-compartment submodel of organic
# > iodine in ‘other tissue’ was replaced by a single compartment called ‘Other 2’ that
# > receives organic iodine from a blood compartment called ‘Blood 2’, and returns most
# > of the organic iodine to Blood 2 but transfers a small fraction as inorganic iodide to
# > Blood 1. The following transfer coefficients are assigned: Blood 2 to Other 2, 15 d⁻¹;
# > Other 2 to Blood 2, 9.0 d⁻¹; and Other 2 to Blood 1, 0.09 d⁻¹. ...
# > (315) ... 第二に、「その他の組織」における有機ヨウ素の2コンパートメントサブモデルは、
# > 「その他2」と呼ばれる単一のコンパートメントに置き換えられました。このコンパートメントは、
# > 「血液2」と呼ばれる血液コンパートメントから有機ヨウ素を受け取り、その大部分を血液2に
# > 戻しますが、少量を無機ヨウ化物として血液1に移行します。以下の移行係数が割り当てられます：
# >   血液2からその他2へ：15 d⁻¹；
# >   その他2から血液2へ：9.0 d⁻¹；
# >   その他2から血液1へ：0.09 d⁻¹。 ...

# (Other3 & Other4) -> Other2
  Blood2                  Other2                   15
  Other2                  Blood2                    9.0
  Other2                  Blood1                    0.09

# ICRP Publ.137 p.62 Para.135
# > (315) ... Iodine atoms are
# > produced at the following sites in the antimony or tellurium models that are not
# > clearly identifiable with specific compartments of the model for iodine: blood compartments,
# > liver compartments, kidneys, thyroid (in the tellurium model), compartments
# > within ‘other soft tissue’, and bone compartments. The following rates of
# > transfer from these compartments to the blood iodide pool of the characteristic
# > model for iodine are assigned: liver compartments or kidneys, 100 d⁻¹ (the rate of
# > loss from the liver iodide and kidney iodide compartments in the characteristic model
# > for iodine); red blood cells, 1000 d⁻¹; other soft tissue or bone surface compartments,
# > 200 d⁻¹ (the highest transfer coefficient to blood in the simplified model for iodine);
# > thyroid, 36 d⁻¹ (the transfer coefficient from the thyroid iodide pool to the blood
# > iodide pool in the characteristic model for iodine); and trabecular and cortical bone
# > volume compartments, the reference rates of trabecular and cortical bone turnover.
# > Iodine atoms produced in the plasma compartment of the antimony or tellurium
# > model are assigned to the blood iodide pool in the characteristic model for iodine.
# > (315) ... ヨウ素原子は、アンチモンモデルまたはテルルモデルにおいて、ヨウ素モデルの特定のコンパートメントと
# > 明確に識別できない以下の部位で生成されます：
# >   血液コンパートメント、肝臓コンパートメント、腎臓、甲状腺（テルルモデルの場合）、
# >   「その他の軟部組織」内のコンパートメント、および骨コンパートメント。
# > これらのコンパートメントからヨウ素特性モデルの血中ヨウ化物プールへの移行速度は、
# > 以下のように割り当てられます：
# >   肝臓コンパートメントまたは腎臓：100 d⁻¹（ヨウ素特性モデルにおける肝臓ヨウ化物および腎臓ヨウ化物コンパートメントからの損失速度）、
# >   赤血球：1000 d⁻¹、
# >   その他の軟組織または骨表面コンパートメント：200 d⁻¹（ヨウ素の簡略化モデルにおける血液への最高移行係数）、
# >   甲状腺：36 d⁻¹（ヨウ素特性モデルにおける甲状腺ヨウ化物プールから血中ヨウ化物プールへの移行係数）、
# >   および海綿骨および皮質骨体積コンパートメント：海綿骨および皮質骨の代謝回転率の基準値。
# > アンチモンまたはテルルモデルの血漿コンパートメントで生成されたヨウ素原子は、
# > ヨウ素特性モデルの血中ヨウ化物プールに割り当てられます。

  {Sb}/Plasma             Blood1                    ---
  {Sb}/RBC                Blood1                 1000
  {Te}/Blood1             Blood1                    ---

  {Sb}/C-bone-S           Blood1                  200
  {Sb}/T-bone-S           Blood1                  200
  {Sb}/C-bone-V           Blood1                $(0.03 / 365)
  {Sb}/T-bone-V           Blood1                $(0.18 / 365)

  {Sb}/Thyroid            Blood1                   36
  {Sb}/Liver1             Blood1                  100
  {Sb}/Liver2             Blood1                  100
  {Te}/Liver              Blood1                  100
  {Sb}/Kidneys            Blood1                  100

  {Sb}/ST0                Blood1                  200
  {Sb}/ST1                Blood1                  200
  {Sb}/ST2                Blood1                  200
  {Te}/ST                 Blood1                  200

$endif
