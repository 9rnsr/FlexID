# ICRP Publ.141 p.235 Para.564 / アクチニド子孫元素としてのフランシウムおよびアスタチン
# > (564) Radioisotopes of francium and astatine appearing in actinide chains considered
# > in this publication have short half-lives, and are assumed to decay at their site
# > of production in systemic tissues or blood.
# > (564) 本書で検討されるアクチニド壊変系列に現れるフランシウムおよびアスタチンの
# > 放射性同位体は半減期が短く、全身組織または血液中の生成部位において崩壊すると仮定されます。

# ICRP Publ.137 p.421 Para.919 / ウラン子孫元素としてのアスタチン
# > (919) The models for actinium, thorium, radium, radon, polonium, lead, bismuth,
# > thallium, francium, and astatine as radium or thorium progeny are applied to these
# > elements as uranium progeny. The models for protactinium and uranium as thorium
# > progeny are applied to protactinium and uranium, respectively, as uranium progeny.
# > (919) ラジウムまたはトリウムの娘核種としての、アクチニウム、トリウム、ラジウム、
# > ラドン、ポロニウム、鉛、ビスマス、タリウム、フランシウム、およびアスタチンのモデルは、
# > ウランの娘核種としてのこれら元素にも適用されます。トリウムの娘核種としてのプロトアクチニウム
# > およびウランのモデルは、ウランの娘核種としてのプロトアクチニウムおよびウランに、
# > それぞれ適用されます。

# ICRP Publ.137 p.370 Para.812 / トリウム子孫元素としてのフランシウムおよびアスタチン
# > (812) The models for actinium, thorium, radium, radon, polonium, lead, bismuth,
# > thallium, francium, and astatine produced systemically by serial decay of members of
# > a thorium chain are the same as the models applied to these elements as progeny of
# > radium (see Section 13). The model for thorium as a radium progeny is applied to
# > protactinium as a thorium progeny based on the similar systemic behaviours of
# > protactinium and thorium in rats (Lanz et al., 1946; Schuppler et al., 1988;
# > Durbin, 2011). A radionuclide produced in a bone volume compartment that is
# > ambiguous with regard to the model for that element (e.g. radium produced in trabecular
# > or cortical bone volume in the thorium model) is assumed to be produced in
# > non-exchangeable bone.
# > (812) トリウム系列の各元素の連続崩壊によって体系的に生成されるアクチニウム、
# > トリウム、ラジウム、ラドン、ポロニウム、鉛、ビスマス、タリウム、フランシウム、
# > およびアスタチンのモデルは、ラジウムの子孫元素としてのこれらの元素に適用される
# > モデルと同じになります（第13節参照）。ラジウムの子孫元素としてのトリウムのモデルは、
# > ラットにおけるプロトアクチニウムとトリウムの類似した全身的挙動に基づいて、
# > トリウムの子孫元素としてのプロトアクチニウムにも適用されます(Lanz et al., 1946;
# > Schuppler et al., 1988; Durbin, 2011)。骨体積コンパートメントで生成される
# > 放射性核種のうち、その元素のモデルに関して曖昧なもの（例えば、トリウムモデルにおいて
# > 骨梁骨または皮質骨体積で生成されるラジウム）は、非交換性骨で生成されると仮定されます。

# ICRP Publ.137 p.334 Para.717 / ラジウム子孫元素としてのフランシウムおよびアスタチン
# > (717) Radioisotopes of francium and astatine appearing in radium chains considered
# > in this publication have half-lives varying from less than 1 s to 22 min.
# > These short-lived radionuclides are assumed to decay at their sites of production in
# > systemic tissues and fluids.
# > (717) 本書で取り上げているラジウムの崩壊系列に現れるフランシウムおよびアスタチンの
# > 放射性同位体は、1秒未満から22分という範囲の半減期を有しています。これらの短寿命
# > 放射性核種は、全身の組織や体液中の生成部位において崩壊すると仮定されます。

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
