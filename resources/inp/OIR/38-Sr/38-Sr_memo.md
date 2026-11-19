# 体内動態モデル間のコンパートメント対応関係

ICRP Publ.134 p.223 Para.482
> (482) Dosimetrically significant radioactive progeny of strontium isotopes considered
> in this publication include isotopes of rubidium, krypton, and yttrium.
> (482) 本報告書で考慮されるストロンチウム同位体の放射性娘核種のうち、線量評価上重要な
> ものには、ルビジウム、クリプトン、およびイットリウムの同位体が含まれる。

38-Srを親とする場合、その系列は{38-Sr, 37-Rb, 36-Kr}と{38-Sr, 39-Y}の2つに大別できる。

|38-Sr         |37-Rb       |36-Kr     |
|--------------|------------|----------|
|Blood         |Plasma      |Blood     |
|              |RBC         |          |
|C-bone-S      |C-bone-S    |          |
|T-bone-S      |T-bone-S    |          |
|Exch-C-bone-V |            |          |
|Exch-T-bone-V |            |          |
|Noch-C-bone-V |            |          |
|Noch-T-bone-V |            |          |
|              |Muscle      |          |

|38-Sr         |39-Y     |
|--------------|---------|
|Blood         |Blood1   |
|              |Blood2   |
|C-bone-S      |C-bone-S |
|T-bone-S      |T-bone-S |
|Exch-C-bone-V |C-bone-V |
|Exch-T-bone-V |T-bone-V |
|Noch-C-bone-V |C-bone-V |
|Noch-T-bone-V |T-bone-V |
|              |Liver0   |
|              |Liver1   |
|              |Kidneys  |

線源領域Otherは、異なる元素のモデル間では同一と識別できないものとして扱う。

|39-Y     |38-Sr         |37-Rb       |
|---------|--------------|------------|
|ST0      |ST0           |OtherTissue |
|ST1      |ST1           |            |
|         |ST2           |            |

# 翻訳メモ

ICRP Publ.134 p.223 Para.482
> (482) Dosimetrically significant radioactive progeny of strontium isotopes considered
> in this publication include isotopes of rubidium, krypton, and yttrium.
> Results of animal studies (Arnold et al., 1955; Lloyd, 1961; Mueller, 1972;
> Stevenson, 1975) indicate that 90Y produced by decay of 90Sr in soft tissues tends
> to migrate from the parent and distribute similarly to intravenously injected yttrium,
> but shows little if any migration from 90Sr when produced in bone volume (see the
> section on yttrium in this publication for summaries of reported data (Section
> 11.2.3.)). No information was found on the behaviour of rubidium produced in
> the body by decay of a strontium parent. The noble gas krypton produced by
> serial decay of strontium and rubidium isotopes presumably migrates from these
> radionuclides over a period of minutes to hours, and escapes from the body to an
> extent determined by the half-life of the krypton isotope.
> ICRP Publ.134 p.223 Para.482
> (482) 本報告書で考慮されるストロンチウム同位体の放射性娘核種のうち、線量評価上重要な
> ものには、ルビジウム、クリプトン、およびイットリウムの同位体が含まれます。動物実験の結果
> （Arnold et al., 1955; Lloyd, 1961; Mueller, 1972; Stevenson, 1975）によれば、軟部組織中で
> 90Srの崩壊により生成された90Yは、親核種から移動し、静脈内投与されたイットリウムと
> 同様の分布を示す傾向があります。一方、骨組織の内部で生成された場合には、90Srからの移動は
> ほとんど、あるいは全く認められません（報告されたデータの概要については、本報告書の
> イットリウムに関する項（セクション11.2.3）を参照のこと）。ストロンチウム（親核種）の
> 崩壊によって体内で生成されるルビジウムの挙動に関する情報は得られませんでした。
> ストロンチウムおよびルビジウム同位体の逐次崩壊によって生成される希ガスである
> クリプトンは、おそらく数分から数時間の時間をかけてこれらの放射性核種から移動し、
> そのクリプトン同位体の半減期によって決まる程度まで体内から放出されると考えられます。

ICRP Publ.134 p.223 Para.483
> (483) The model used in this publication for yttrium as a progeny of strontium is
> based on the model for yttrium as a parent described elsewhere in this publication,
> but additional assumptions are made to address structural differences in the strontium
> and yttrium models. Yttrium produced in a compartment of bone is assumed to
> follow the same kinetics as if deposited in the compartment as a parent radionuclide.
> No distinction is made between the exchangeable and non-exchangeable bone
> volume compartments of the strontium model when applied to yttrium, i.e. each
> compartment is treated simply as the bone volume compartment for the corresponding
> bone type in the yttrium model. Yttrium produced in a soft tissue compartment
> of the strontium model (ST0, ST1, or ST2) is assumed to transfer to blood with a
> half-time of 3 d (the shortest removal half-time from compartments of other soft
> tissue in the model for yttrium as a parent), and then to follow the kinetics of yttrium
> as a parent radionuclide.
> (483) 本報告書において、ストロンチウムの娘核種であるイットリウムに対して用いられるモデルは、
> 同じく本報告書内で記述されている「親核種としてのイットリウム」のモデルに基づいていますが、
> ストロンチウムのモデルとイットリウムのモデルとの間の構造的な差異に対応するため、追加の
> 仮定が設けられています。骨のコンパートメント内で生成されたイットリウムは、親放射性核種として
> そのコンパートメントに沈着した場合と同様の動態を示すものと仮定されます。ストロンチウムの
> モデルにおける「交換可能」および「交換不可能」な骨体積コンパートメントの区別は、
> イットリウムに適用する際には行われません。すなわち、各コンパートメントは、イットリウムの
> モデルにおける対応する骨タイプの「骨体積コンパートメント」として単純に扱われます。
> ストロンチウムのモデルの軟部組織コンパートメント（ST0、ST1、またはST2）内で生成された
> イットリウムは、3日の半減期（親核種としてのイットリウムのモデルにおける他の軟部組織
> コンパートメントからの最短の除去半減期）で血液へ移行し、その後、親放射性核種としての
> イットリウムの動態に従うものと仮定されます。

ICRP Publ.134 p.224 Para.484
> (484) The model for rubidium as a progeny of strontium is a condensed version of
> a proposed model for rubidium as a parent radionuclide (Leggett and Williams,
> 1988). The model is based on the same principles as the model for caesium, a chemical
> and physiological analogue of rubidium, described elsewhere in the OIR series.
> That is, the biokinetics of systemic rubidium is predicted on the basis of the distribution
> of cardiac output, experimentally determined tissue-specific extraction fractions,
> and the steady-state distribution of stable rubidium in the body. The reference
> division of cardiac output in the adult male tabulated in Publication 89 (ICRP, 2002)
> is applied here. The present version of the model depicts blood plasma as a central
> compartment that exchanges rubidium with RBCs, trabecular bone surface, cortical
> bone surface, muscle, and a compartment representing all other soft tissue. Rates of
> transfer of rubidium from plasma are as follows: 6 d⁻¹ to RBCs, 255 d⁻¹ to muscle,
> 5.6 d⁻¹ to cortical bone surface, 8.4 d⁻¹ to trabecular bone surface, 855 d⁻¹ to other
> tissue, 3.9 d⁻¹ to urinary bladder contents, 1.2 d⁻¹ to right colon contents, and
> 0.1 d⁻¹ to excreta (loss in sweat). Transfer rates from RBCs or tissues to plasma
> are as follows: 0.35 d⁻¹ from RBCs, 1.14 d⁻¹ from muscle, 1.68 d⁻¹ from bone surface
> compartments, and 10.3 d⁻¹ from other tissue. Rubidium produced by decay of
> strontium in blood is assigned to plasma. Rubidium produced in exchangeable or
> non-exchangeable bone volume compartments of the strontium model are transferred
> to plasma at the rate of bone turnover. Rubidium produced in soft tissue
> compartments of the strontium model (ST0, ST1, or ST2) are transferred to
> plasma at a rate of 10.3 d⁻¹. The subsequent behaviour of rubidium that reaches
> plasma is determined by the model for rubidium described above.
> (484) ストロンチウムの娘核種としてのルビジウムに関するモデルは、親放射性核種としての
> ルビジウムについて提案されたモデル（Leggett and Williams, 1988）を簡略化したものです。
> このモデルは、OIRシリーズの他の箇所で記述されているルビジウムの化学的・生理学的類似体である
> セシウムのモデルと同じ原理に基づいています。すなわち、全身におけるルビジウムの生物学的動態は、
> 心拍出量の分配、実験的に決定された組織特異的な抽出率、および体内における安定ルビジウムの
> 定常状態での分布に基づいて予測されます。ここでは、Publication 89（ICRP, 2002）に示された
> 成人男性における心拍出量の基準分配が適用されます。本モデルでは、血漿を中心コンパートメントとし、
> そこから赤血球、海綿骨表面、皮質骨表面、筋肉、およびその他のすべての軟部組織を表す
> コンパートメントとの間でルビジウムの交換が行われるものとしています。
> 血漿から各コンパートメントへのルビジウムの移行速度は以下の通りです：
> 赤血球へ6 d⁻¹、筋肉へ255 d⁻¹、皮質骨表面へ5.6 d⁻¹、海綿骨表面へ8.4 d⁻¹、その他の組織へ855 d⁻¹、
> 膀胱内容物へ3.9 d⁻¹、右結腸内容物へ1.2 d⁻¹、および排泄物（汗による損失）へ0.1 d⁻¹。
> 赤血球または各組織から血漿への移行速度は以下の通りです：赤血球から0.35 d⁻¹、
> 筋肉から1.14 d⁻¹、骨表面コンパートメントから1.68 d⁻¹、およびその他の組織から10.3 d⁻¹。
> 血液中のストロンチウムの崩壊により生成されたルビジウムは血漿に移行します。
> ストロンチウムモデルの交換性または非交換の骨体積コンパートメントで生成された
> ルビジウムは、骨代謝の速度で血漿へ移行します。ストロンチウムモデルの軟部組織コンパートメント
> （ST0、ST1、またはST2）で生成されたルビジウムは、10.3 d⁻¹の速度で血漿へ移行します。
> 血漿に到達したルビジウムのその後の挙動は、上記のルビジウムモデルによって決定されます。

ICRP Publ.134 p.224 Para.485
> (485) The model for krypton produced by serial decay of strontium and rubidium
> in systemic compartments is similar to the model applied in the OIR series to radon
> produced in vivo by decay of a parent radionuclide (ICRP, 2017, see radon section in
> OIR Part 3). Krypton is assumed to follow the bone model for radon introduced in
> Publication 67 (ICRP, 1993), but is assigned a higher rate of removal from soft tissues
> to blood than is assumed for radon. Specifically, krypton produced in
> non-exchangeable bone volume, exchangeable bone volume, or bone surface transfers
> to blood at rates of 0.36 d⁻¹, 1.5 d⁻¹, or 100 d⁻¹, respectively. Krypton produced in a
> soft tissue compartment transfers to blood with a half-time of 15 min, compared with
> an assumed half-time of 30 min for radon produced by radioactive decay in soft
> tissues. Krypton entering blood is assumed to be removed from the body (exhaled)
> at a rate of 1000 d⁻¹, corresponding to a half-time of 1 min. Recycling of krypton to
> tissues via arterial blood is not depicted explicitly but is considered in the assignment
> of effective half-times in tissues. The model is intended to yield a conservative average
> residence time of krypton atoms produced in systemic pools by decay of a parent
> radionuclide. It is recognised that the residence time of krypton in the body following
> production in tissues depends on the distribution of the parent radionuclide.
> (485) 全身コンパートメント内でのストロンチウムおよびルビジウムの逐次壊変によって生成される
> クリプトンに関するモデルは、親放射性核種の壊変により生体内で生成されるラドンに対して
> OIRシリーズで適用されたモデルと類似しています（ICRP, 2017；OIRパート3のラドンの項を参照）。
> クリプトンは、Publication 67（ICRP, 1993）で導入されたラドンに関する骨モデルに従うと
> 仮定されていますが、軟部組織から血液への除去速度については、ラドンに対して仮定された値よりも
> 高い値が割り当てられています。具体的には、非交換性骨領域、交換性骨領域、または骨表面で
> 生成されたクリプトンは、それぞれ0.36 d⁻¹、1.5 d⁻¹、100 d⁻¹の速度で血液へ移行します。
> 軟部組織コンパートメントで生成されたクリプトンは、半減期15分で血液へ移行するのに対し、
> 軟部組織内での放射性壊変によって生成されるラドンの場合は、半減期30分と仮定されています。
> 血液に入ったクリプトンは、半減期1分に相当する1000 d⁻¹の速度で体内から除去（呼気により排出）されると
> 仮定されています。動脈血を介した組織へのクリプトンの再循環は明示的には示されていませんが、
> 組織における実効半減期を割り当てる際に考慮されています。このモデルは、親放射性核種の壊変により
> 全身プール内で生成されたクリプトン原子の平均滞留時間について、保守的な値を与えることを
> 意図しています。組織内での生成後に体内にとどまるクリプトンの滞留時間は、親放射性核種の
> 分布に依存することが認識されています。
