# ICRP Publ.141 p.235 Para.564 / アクチニド子孫元素としてのフランシウムおよびアスタチン
# > (564) Radioisotopes of francium and astatine appearing in actinide chains considered
# > in this publication have short half-lives, and are assumed to decay at their site
# > of production in systemic tissues or blood.
# > (564) 本書で検討されるアクチニド壊変系列に現れるフランシウムおよびアスタチンの
# > 放射性同位体は半減期が短く、全身組織または血液中の生成部位において崩壊すると仮定されます。

[{At}:compartment]
#-----+---------------------| S-Coefficient
# Func| Compartment         | Source Region
#-----+---------------------+---------------
  acc   Blood                 Blood


[{At}:transfer]
#-----------------------+---------------------+--------------------
# From                  | To                  | Transfer Coef.[/d]
#-----------------------+---------------------+--------------------

  {*}/Blood               Blood                     ---
  {Pu}/Blood0             Blood                  1000
  {Pu}/Blood1             Blood                  1000
  {Pu}/Blood2             Blood                  1000
  {*}/Plasma              Blood                  1000
  {Po}/Plasma1            Blood                  1000
  {Po}/Plasma2            Blood                  1000
  {Po}/Plasma3            Blood                  1000
  {*}/RBC                 Blood                  1000
