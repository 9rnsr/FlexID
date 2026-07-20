# 体内動態モデル間のコンパートメント対応関係

|96-Cm    |95-Am    |94-Pu     |93-Np    |92-U          |91-Pa    |90-Th    |89-Ac    |88-Ra         |87-Fr |86-Rn  |85-At |84-Po    |83-Bi    |82-Pb         |81-Tl    |
|---------|---------|----------|---------|--------------|---------|---------|---------|--------------|------|-------|------|---------|---------|--------------|---------|
|         |         |Blood0    |         |              |         |         |         |              |   *2 |       |   *2 |         |         |              |         |
|         |         |Blood1    |         |              |         |         |         |              |   *2 |       |   *2 |         |         |              |         |
|         |         |Blood2    |         |              |         |         |         |              |   *2 |       |   *2 |         |         |              |         |
|Blood    |Blood    |          |Blood    |              |Blood    |Blood    |Blood    |Blood         |   *2 |Blood  |   *2 |         |         |              |         |
|         |         |          |         |              |         |         |         |              |   *2 |       |   *2 |Plasma1  |         |              |         |
|         |         |          |         |              |         |         |         |              |   *2 |       |   *2 |Plasma2  |         |              |         |
|         |         |          |         |              |         |         |         |              |   *2 |       |   *2 |Plasma3  |         |              |         |
|         |         |          |         |Plasma        |         |         |         |              |   *2 |       |   *2 |         |Plasma   |Plasma        |Plasma   |
|         |         |          |         |RBC           |         |         |         |              |   *2 |       |   *2 |RBC      |RBC      |RBC           |RBC      |
|C-bone-S |C-bone-S |C-bone-S  |C-bone-S |C-bone-S      |C-bone-S |C-bone-S |C-bone-S |C-bone-S      |   *2 |       |   *2 |C-bone-S |C-bone-S |C-bone-S      |C-bone-S |
|T-bone-S |T-bone-S |T-bone-S  |T-bone-S |T-bone-S      |T-bone-S |T-bone-S |T-bone-S |T-bone-S      |   *2 |       |   *2 |T-bone-S |T-bone-S |T-bone-S      |T-bone-S |
|         |         |          |         |Exch-C-bone-V |         |         |         |Exch-C-bone-V |   *2 |       |   *2 |         |         |Exch-C-bone-V |         |
|         |         |          |         |Exch-T-bone-V |         |         |         |Exch-T-bone-V |   *2 |       |   *2 |         |         |Exch-T-bone-V |         |
|C-bone-V |C-bone-V |C-bone-V  |C-bone-V |Noch-C-bone-V |C-bone-V |C-bone-V |C-bone-V |Noch-C-bone-V |   *2 |       |   *2 |         |         |Noch-C-bone-V |         |
|T-bone-V |T-bone-V |T-bone-V  |T-bone-V |Noch-T-bone-V |T-bone-V |T-bone-V |T-bone-V |Noch-T-bone-V |   *2 |       |   *2 |         |         |Noch-T-bone-V |         |
|C-marrow |C-marrow |C-marrow  |C-marrow |C-marrow      |C-marrow |C-marrow |C-marrow |C-marrow      |   *2 |       |   *2 |C-marrow |C-marrow |C-marrow      |C-marrow |
|T-marrow |T-marrow |T-marrow  |T-marrow |T-marrow      |T-marrow |T-marrow |T-marrow |T-marrow      |   *2 |       |   *2 |T-marrow |T-marrow |T-marrow      |T-marrow |
|         |         |Liver0    |         |              |         |         |         |              |   *2 |       |   *2 |         |         |              |         |
|         |         |Liver1    |         |              |         |         |         |              |   *2 |       |   *2 |         |         |              |         |
|         |         |Liver2    |         |              |         |         |         |              |   *2 |       |   *2 |         |         |              |         |
|Liver1   |Liver1   |          |Liver1   |Liver1        |Liver1   |Liver1   |Liver1   |Liver1        |   *2 |       |   *2 |Liver1   |Liver1   |Liver1        |Liver    |
|Liver2   |Liver2   |          |Liver2   |Liver2        |Liver2   |Liver2   |Liver2   |Liver2        |   *2 |       |   *2 |Liver2   |Liver2   |Liver2        |         |
|Kidneys1 |Kidneys1 |Kidneys1  |Kidneys1 |Kidneys1      |Kidneys1 |Kidneys1 |Kidneys1 |Kidneys1      |   *2 |       |   *2 |Kidneys1 |Kidneys1 |Kidneys1      |         |
|Kidneys2 |Kidneys2 |Kidneys2  |Kidneys2 |Kidneys2      |Kidneys2 |Kidneys2 |Kidneys2 |Kidneys2      |   *2 |       |   *2 |Kidneys2 |Kidneys2 |Kidneys2      |Kidneys  |
|Testes   |Testes   |Testes    |Testes   |Testes        |Testes   |Testes   |Testes   |Testes        |   *2 |       |   *2 |Testes   |Testes   |Testes        |Testes   |
|Ovaries  |Ovaries  |Ovaries   |Ovaries  |Ovaries       |Ovaries  |Ovaries  |Ovaries  |Ovaries       |   *2 |       |   *2 |Ovaries  |Ovaries  |Ovaries       |Ovaries  |
|Skin     |Skin     |Skin   *1 |Skin     |Skin          |Skin     |Skin     |Skin     |Skin          |   *2 |       |   *2 |Skin     |Skin     |Skin          |Skin     |
|Spleen   |Spleen   |Spleen *1 |Spleen   |Spleen        |Spleen   |Spleen   |Spleen   |Spleen        |   *2 |       |   *2 |Spleen   |Spleen   |Spleen        |Spleen   |

*1 子孫核種の場合のみ追加する
*2 壊変経路の通過点として自動生成されるコンパートメント

軟組織は異なる元素の全身モデル間では同一と識別できないものとして扱う。

|96-Cm    |95-Am    |94-Pu     |93-Np    |92-U          |91-Pa    |90-Th    |89-Ac    |88-Ra         |87-Fr |86-Rn  |85-At |84-Po    |83-Bi    |82-Pb         |81-Tl    |
|---------|---------|----------|---------|--------------|---------|---------|---------|--------------|------|-------|------|---------|---------|--------------|---------|
|ST0      |ST0      |ST0       |ST0      |ST0           |ST0      |ST0      |ST0      |ST0           |      |       |      |Other    |ST0      |ST0           |Other    |
|ST1      |ST1      |ST1       |ST1      |ST1           |ST1      |ST1      |ST1      |ST1           |      |       |      |         |ST1      |ST1           |         |
|ST2      |ST2      |ST2       |ST2      |ST2           |ST2      |ST2      |ST2      |ST2           |      |       |      |         |ST2      |ST2           |         |


# 翻訳メモ

> ICRP Publ.141 p.341 Para.856
> (856) The treatment of radioactive progeny of plutonium produced in systemic
> compartments or absorbed to blood after production in the respiratory or gastrointestinal
> tract is described in Section 18.2.4.
> (856) 全身の区画内で生成されたプルトニウムの放射性娘核種、あるいは呼吸器や消化管で生成された後に
> 血液中に吸収されたプルトニウムの放射性娘核種の取り扱いについては、18.2.4節に記載されている。


> ICRP Publ.141 p.232 Para.561
> (561) Chain members addressed in the derivation of dose coefficients for the actinides
> addressed in this publication include isotopes of thallium, lead, bismuth, polonium,
> astatine, radon, francium, radium, actinium, thorium, protactinium, uranium,
> neptunium, plutonium, americium, curium, berkelium, californium, einsteinium, and
> fermium.
> (561) 本書においてアクチノイドの線量係数の導出対象とされた崩壊系列の核種には、
> タリウム、鉛、ビスマス、ポロニウム、アスタチン、ラドン、フランシウム、ラジウム、
> アクチニウム、トリウム、プロトアクチニウム、ウラン、ネプツニウム、プルトニウム、
> アメリシウム、キュリウム、バークリウム、カリホルニウム、アインスタイニウム、および
> フェルミウムの同位体が含まれる。

> ICRP Publ.141 p.232 Para.562 / アクチノイドの子孫としての（タリウム、鉛、ビスマス、ポロニウム、ラジウム）は、ラジウム子孫としてのモデルを用いる。
> ICRP Publ.141 p.232 Para.562
> (562) The models applied here to thallium, lead, bismuth, polonium, and radium
> as actinide progeny are the models applied to these elements as progeny of radium
> [described in OIR Part 3 (ICRP, 2017)]. The model applied here to uranium as an
> actinide progeny is the model applied to uranium as a progeny of thorium [also
> described in OIR Part 3 (ICRP, 2017)].
> (562) ここでタリウム、鉛、ビスマス、ポロニウム、およびラジウムに適用されるモデルは、
> アクチニドの娘核種としてこれらの元素に適用されるモデルと同じである [OIR Part 3 (ICRP, 2017)に記載されている]。
> ここでウランをアクチニドの娘核種として扱う場合に適用するモデルは、ウランをトリウムの
> 娘核種として扱う場合に適用するモデルと同じである [同じくOIR Part 3(ICRP, 2017)に記載されている]。

> ICRP Publ.141 p.235 Para.563
> (563) The model applied here to radon as an actinide progeny is a generic model
> applied in the OIR series to radon produced by radioactive decay in a systemic
> compartment. Radon produced in a compartment identified as non-exchangeable
> bone volume, exchangeable bone volume, or bone surface transfers to blood at the
> rate 0.36, 1.5, or 100 d⁻¹, respectively; radon produced in a compartment identified
> simply as ‘bone volume’ transfers to blood at 0.36 d⁻¹; radon produced in a soft
> tissue compartment transfers to blood at 33.3 d⁻¹; and radon produced in blood or
> entering blood is removed from the body (exhaled) at 1000 d⁻¹.
> (563) アクチニドの娘核種であるラドンに対してここで適用されるモデルは、全身性
> コンパートメントにおける放射性崩壊によって生成されるラドンに対し、OIRシリーズで
> 適用されている汎用モデルである。非交換性骨容積、交換性骨容積、または骨表面として
> 特定されるコンパートメントで生成されたラドンは、それぞれ0.36、1.5、または100 d⁻¹の
> 速度で血液へ移行する。単に「骨容積」として特定されるコンパートメントで生成されたラドンは
> 0.36 d⁻¹の速度で血液へ移行し、軟部組織コンパートメントで生成されたラドンは33.3 d⁻¹の
> 速度で血液へ移行する。また、血液中で生成された、あるいは血液中に流入したラドンは、
> 1000 d⁻¹の速度で体内から除去（呼気により排出）される。

> ICRP Publ.141 p.235 Para.564
> (564) Radioisotopes of francium and astatine appearing in actinide chains considered
> in this publication have short half-lives, and are assumed to decay at their site
> of production in systemic tissues or blood.
> (564) 本報告書で検討されるアクチニド壊変系列に現れるフランシウムおよびアスタチンの
> 放射性同位体は半減期が短く、全身組織または血液中の生成部位において崩壊すると仮定される。

> ICRP Publ.141 p.235 Para.565
> (565) The model applied here to thorium as an actinide progeny is the model
> applied in OIR Part 3 (ICRP, 2017) to thorium as a progeny of radium. Briefly,
> two compartments, one representing the spleen and the other representing the skin,
> are added to the explicitly identified source regions in the characteristic model for
> thorium described in OIR Part 3. The spleen and skin are assumed to receive 0.5%
> and 2%, respectively, of thorium leaving the circulation, and to return thorium to
> blood with a biological half-time of 2 y. Thorium produced in a compartment that is
> not identifiable with a compartment in the thorium model is assumed to transfer to
> blood at the following rates: 1000 d⁻¹ if produced in blood; 0.462 d⁻¹ if produced in
> soft tissue; and at the rate of bone turnover if produced in a bone volume
> compartment.
> (565) ここでアクチニドの娘核種としてのトリウムに適用されるモデルは、OIR Part 3 
> (ICRP, 2017) においてラジウムの娘核種としてのトリウムに適用されたモデルである。
> 簡潔に言えば、OIR Part 3に記載されているトリウムの特性モデルにおける明示的な
> 線源領域に対し、脾臓を表すコンパートメントと皮膚を表すコンパートメントの2つが
> 追加されている。脾臓および皮膚は、循環系から移行するトリウムのそれぞれ0.5%および
> 2%を受け取り、生物学的半減期2年でトリウムを血液へ戻すと仮定される。トリウムモデル内の
> コンパートメントとして特定できないコンパートメント内で生成されたトリウムは、
> 以下の速度で血液へ移行すると仮定される：血液中で生成された場合は1000 d⁻¹、
> 軟部組織で生成された場合は0.462 d⁻¹、そして骨容積コンパートメントで生成された
> 場合は骨代謝回転の速度。

> ICRP Publ.141 p.235 Para.566
> (566) The models applied here to actinium, protactinium, neptunium, plutonium,
> americium, curium, berkelium, californium, einsteinium, and fermium as actinide
> progeny are modifications of their characteristic models described earlier in this
> section. For a given element in this group, two compartments representing the
> skin and spleen are added to the explicitly identified source regions in the element’s
> characteristic model. These compartments are taken from the intermediate soft tissue
> compartment, ST1; that is, the deposition fraction for ST1 is reduced by the deposition
> fractions assigned to the spleen and skin, and the removal half-time from ST1 is
> assigned to these added compartments. Deposition of the element in the skin is
> calculated as its mass fraction of other soft tissue times its deposition fraction in
> other soft tissue excluding the rapid-turnover compartment, ST0. The deposition
> fraction for the spleen is set at one-third of the deposition fraction for the skin,
> considering the relative masses of these tissues and the typically higher concentrations
> of the actinides in the spleen than the skin observed in laboratory animals and
> humans. If the element is produced in a compartment that is not identifiable with a
> compartment in its characteristic model, it is assumed to transfer to the element’s
> blood compartment [Blood 1 in the case of plutonium, which has multiple blood
> compartments (see Table 18.6)] at the rate 1000 d⁻¹ if produced in a blood compartment,
> at the rate of transfer from the fast-turnover soft tissue compartment ST0 to
> blood if produced in a soft tissue compartment, and at the rate of bone turnover if
> produced in a bone volume compartment.
> (566) アクチニドの娘核種であるアクチニウム、プロトアクチニウム、ネプツニウム、
> プルトニウム、アメリシウム、キュリウム、バークリウム、カリホルニウム、
> アインスタイニウム、およびフェルミウムに対してここで適用されるモデルは、
> 本節の冒頭で述べた各元素の特性モデルを修正したものである。このグループの特定の元素に
> ついて、その特性モデルにおいて明示的に特定された線源領域に加え、皮膚と脾臓を表す2つの
> コンパートメントが追加される。これらのコンパートメントは、中間軟部組織コンパートメント
> ST1から切り出されたものである。すなわち、ST1の沈着分率は、脾臓および皮膚に割り当てられた
> 沈着分率の分だけ減らされ、ST1からの除去半減期が、これら追加されたコンパートメントに
> 割り当てられる。皮膚への元素の沈着量は、「その他の軟部組織」に対する質量の割合に、
> 急速代謝コンパートメントST0を除いた「その他の軟部組織」における沈着分率を乗じることで
> 算出される。脾臓の沈着分率は、皮膚の沈着分率の3分の1に設定される。これは、これらの
> 組織の相対的な質量、および実験動物やヒトにおいて皮膚よりも脾臓でアクチニドの濃度が
> 高くなる傾向が観察されていることを考慮したものである。もし元素が、その特性モデル上の
> コンパートメントと同一視できないコンパートメント内で生成された場合、その元素の
> 血液コンパートメント（複数の血液コンパートメントを持つプルトニウムの場合はBlood 1。
> 表18.6参照）へ移行すると仮定される。その際の移行速度は、血液コンパートメント内で
> 生成された場合は1000 d⁻¹、軟部組織コンパートメント内で生成された場合は急速代謝
> 軟部組織コンパートメントST0から血液への移行速度、そして骨容積コンパートメント内で
> 生成された場合は骨代謝回転速度となる。

> ICRP Publ.141 p.236 Para.567
> (567) The model for plutonium as a progeny is further modified by removing the
> transfers from blood to ST0 and from blood to Blood 1 (Table 18.6), and adding a
> transfer of 0.33 d⁻¹ from Blood 1 to ST0. This simplifies the model for plutonium as
> a progeny by eliminating a blood compartment (‘Blood 0’ in Table 18.6). The added
> transfer coefficient of 0.33 d⁻¹ from Blood 1 to ST0 implies that ST0 receives 30% of
> plutonium leaving Blood 1. Total deposition in ST0 is virtually the same as in the
> model for plutonium as a parent, but the rate of accumulation of plutonium in ST0 is
> substantially lower in this simplified version of the model.
> (567) 娘核種としてのプルトニウムに関するモデルは、血液からST0への移行および血液から
> Blood 1への移行（表18.6）を削除し、Blood 1からST0への0.33 d⁻¹の移行を追加することに
> よって、さらに修正される。これにより、血液コンパートメント（表18.6における「Blood 0」）が
> 排除され、娘核種としてのプルトニウムのモデルが簡素化される。Blood 1からST0への0.33 d⁻¹
> という追加された移行係数は、Blood 1から流出するプルトニウムの30%がST0に移行することを
> 意味する。ST0における総沈着量は、親核種としてのプルトニウムのモデルにおける量と実質的に
> 同じであるが、ST0におけるプルトニウムの蓄積速度は、この簡素化されたモデルでは大幅に
> 低くなっている。


アクチノイド(プルトニウム以外)の全身の移行係数

	# ICRP Publ.141 p.234 Table 18.9
	Path					Transfer coefficient (d^-1)
	From		To			Ac			Pa,Th*		Np			Am,Cm		Bk			Cf			Es,Fm
	Blood		Liver 1		11.6		0.097		0.194		11.6		0.194		2.33		1.75
	Blood		Trab surf	3.49		0.679		0.480		3.49		0.129		2.91		3.20
	Blood		Cort surf	3.49		0.679		0.393		3.49		0.129		2.91		3.20
	Blood		Kidneys 1	0.466		0.0679		0.0291		0.466		0.0129		0.233		0.116
	Blood		Kidneys 2	0.116		0.0194		0.0097		0.116		0.00647		0.116		0.0582
	Blood		UB cont		1.63		0.107		0.621		1.63		0.0582		1.28		1.51
	Blood		RC cont		0.303		0.0097		0.0136		0.303		0.0388		0.699		0.699
	Blood		Testes		0.0082		0.00068		0.00068		0.0082		0.00023		0.00408		0.00408
	Blood		Ovaries		0.0026		0.00021		0.00021		0.0026		0.00007		0.00128		0.00128
	Blood		ST0			10.0		0.832		0.832		10.0		0.277		4.99		4.99
	Blood		ST1			1.67		0.243		0.161		1.67		0.0647		0.926		0.868
	Blood		ST2			0.466		0.0388		0.0388		0.466		0.0129		0.233		0.233
	Liver 1		SI cont		0.0006		0.000475	0.000133	0.0006		0.0006		0.0006		0.0006
	Liver 1		Liver 2		0.0225		0.00095		0.00177		0.0225		0.0225		0.0225		0.0225
	Liver 1		Blood		0			0.000475	0			0			0			0			0
	Liver 2		Blood		0.0019		0.000211	0.0019		0.0019		0.0019		0.0019		0.0019
	Trab surf	Trab mar	4.93E-4		4.93E-4		4.93E-4		4.93E-4		4.93E-4		4.93E-4		4.93E-4
	Trab surf	Trab vol	2.47E-4		2.47E-4		2.47E-4		2.47E-4		2.47E-4		2.47E-4		2.47E-4
	Trab vol	Trab mar	4.93E-4		4.93E-4		4.93E-4		4.93E-4		4.93E-4		4.93E-4		4.93E-4
	Trab mar	Blood		0.0076		0.0076		0.0076		0.0076		0.0076		0.0076		0.0076
	Cort surf	Cort mar	8.21E-5		8.21E-5		8.21E-5		8.21E-5		8.21E-5		8.21E-5		8.21E-5
	Cort surf	Cort vol	4.11E-5		4.11E-5		4.11E-5		4.11E-5		4.11E-5		4.11E-5		4.11E-5
	Cort vol	Cort mar	8.21E-5		8.21E-5		8.21E-5		8.21E-5		8.21E-5		8.21E-5		8.21E-5
	Cort mar	Blood		0.0076		0.0076		0.0076		0.00253		0.0076		0.0076		0.0076
	Cort mar	Cort surf	0			0			0			0.00507		0			0			0
	Kidneys 1	UB cont		0.099		0.0462		0.0495		0.099		0.099		0.099		0.099
	Kidneys 2	Blood		0.00139		0.00038		0.00139		0.00139		0.00038		0.00038		0.00038
	Testes		Blood		0.00038		0.00038		0.00038		0.00038		0.00038		0.00038		0.00038
	Ovaries		Blood		0.00038		0.00038		0.00038		0.00038		0.00038		0.00038		0.00038
	ST0			Blood		1.39		0.462		0.693		1.39		1.39		1.39		1.39
	ST1			Blood		0.0139		0.00095		0.00693		0.0139		0.00693		0.00693		0.00693
	ST2			Blood		1.9E-5		1.9E-5		1.9E-5		1.9E-5		1.9E-5		1.9E-5		1.9E-5
