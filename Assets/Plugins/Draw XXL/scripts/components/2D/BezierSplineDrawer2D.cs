﻿namespace DrawXXL
{
    using UnityEngine;
    using System.Collections.Generic;

    [HelpURL("https://www.symphonygames.net/drawxxldocumentation/index.html")]
    [AddComponentMenu("Draw XXL/2D/Bezier Spline Drawer 2D")]
    [DefaultExecutionOrder(31000)] //negative numers are early, positive numbers are late. Range is till 32000 to both negative and positive direction.
    public class BezierSplineDrawer2D : VisualizerParent
    {
        static Color default_colorOfLinesAlongZToBoundGameobjects = new Color(0.09f, 1.0f, 0.653f, 1.0f);

        [SerializeField] public bool drawSpaceSection_isOutfolded = false; //is only "public" to silence the compiler warning saying that it is "never used". The compiler doesn't know that it is used via serialization.
        [SerializeField] public BezierSplineDrawer.DrawSpace drawSpace;
        [SerializeField] bool keepWorldPos_duringDrawSpaceChange = false;

        [SerializeField] Color color = DrawBasics.defaultColor;
        [SerializeField] public Color color_ofAnchorPoints = BezierSplineDrawer.default_color_ofAnchorPoints;
        [SerializeField] public Color color_ofHelperPoints = BezierSplineDrawer.default_color_ofHelperPoints;
        [SerializeField] float lineWidth = 0.0f;
        [SerializeField] public bool gapFromEndToStart_isClosed = false;
        [SerializeField] int straightSubDivisionsPerSegment = 50;
        float textSize = 0.1f;

        [SerializeField] public BezierSplineDrawer.PositionHandleOrientation_forEditorPivotIsGlobal_butDrawSpaceIsLocal positionHandleOrientation_forEditorPivotIsGlobal_butDrawSpaceIsLocal = BezierSplineDrawer.PositionHandleOrientation_forEditorPivotIsGlobal_butDrawSpaceIsLocal.localDrawSpace;

        public static float default_handleSizeOf_customHandle_atAnchors = 0.28f;
        static float default_handleSizeOf_customHandle_atHelpers = 0.18f;
        static float default_handleSizeOf_plusButtons = 0.2f;

        [SerializeField] public bool handlesSection_isOutfolded = false;
        [SerializeField] public bool hideAllHandles = false;
        [SerializeField] public bool showHandleFor_position_atAnchors = true;
        [SerializeField] public bool showHandleFor_position_atHelpers = true;
        [SerializeField] [Range(0.2f, 2.0f)] public float handleSizeFor_position_atAnchors = 0.85f;
        [SerializeField] [Range(0.2f, 2.0f)] public float handleSizeFor_position_atHelpers = 0.85f;
        [SerializeField] public bool showHandleFor_rotation = true;
        [SerializeField] [Range(0.15f, 1.5f)] public float handleSizeFor_rotation = 0.55f;
        [SerializeField] public bool showCustomHandleFor_anchorPoints = true;
        [SerializeField] public bool showCustomHandleFor_helperPoints = true;
        [SerializeField] [Range(0.08f, 0.75f)] public float handleSizeOf_customHandle_atAnchors = default_handleSizeOf_customHandle_atAnchors;
        [SerializeField] [Range(0.08f, 0.75f)] public float handleSizeOf_customHandle_atHelpers = default_handleSizeOf_customHandle_atHelpers;
        [SerializeField] public bool showHandleFor_plusButtons_atSplineStartAndEnd = true;
        [SerializeField] public bool showHandleFor_plusButtons_insideSegments = true;
        [SerializeField] [Range(0.08f, 0.75f)] public float handleSizeOf_plusButtons = default_handleSizeOf_plusButtons;
        [SerializeField] public bool showDottedLinesAlongZToBoundGameobjects = true;
        [SerializeField] public Color colorOfLinesAlongZToBoundGameobjects = default_colorOfLinesAlongZToBoundGameobjects;
        [SerializeField] public float dotLength_ofDottedLinesAlongZToBoundGameobjects = 8.0f;

        [SerializeField] public bool controlPointsList_isOutfolded = false; //is only "public" to silence the compiler warning saying that it is "never used". The compiler doesn't know that it is used via serialization.
        [SerializeField] public bool defaultPosRotWeightOffsetOfNewlyCreatedPoints_section_isOutfolded = false; //is only "public" to silence the compiler warning saying that it is "never used". The compiler doesn't know that it is used via serialization.
        [SerializeField] public bool defaultPosOffsetOfNewlyCreatedPoints_subSection_isOutfolded = false; //is only "public" to silence the compiler warning saying that it is "never used". The compiler doesn't know that it is used via serialization.
        [SerializeField] public bool defaultRotOfNewlyCreatedPoints_subSection_isOutfolded = false; //is only "public" to silence the compiler warning saying that it is "never used". The compiler doesn't know that it is used via serialization.
        [SerializeField] public bool defaultWeightsOfNewlyCreatedPoints_subSection_isOutfolded = false; //is only "public" to silence the compiler warning saying that it is "never used". The compiler doesn't know that it is used via serialization.

        [SerializeField] public BezierSplineDrawer.DefinitionType_ofDefaultPosOffset definitionType_ofDefaultPosOffset = BezierSplineDrawer.DefinitionType_ofDefaultPosOffset.straightExtentionOfCurveEnd;
        [SerializeField] public float distanceBetweenTriplets_forNewlyCreatedTriplets_caseOf_straightExtentionOfCurveEnd_inUnitsOfActiveDrawSpace = 3.0f;

        [SerializeField] public BezierSplineDrawer.DefinitionType_ofDefaultRot definitionType_ofDefaultRot = BezierSplineDrawer.DefinitionType_ofDefaultRot.sameAsCurveEnd;

        [SerializeField] public float forwardWeightDistance_ofNewlyCreatedPoints_inUnitsOfActiveDrawSpace = BezierSplineDrawer.default_forwardWeightDistance_ofNewlyCreatedPoints;
        [SerializeField] public float backwardWeightDistance_ofNewlyCreatedPoints_inUnitsOfActiveDrawSpace = BezierSplineDrawer.default_backwardWeightDistance_ofNewlyCreatedPoints;
        [SerializeField] public InternalDXXL_BezierControlAnchorSubPoint.JunctureType junctureType_ofNewlyCreatedPoints = InternalDXXL_BezierControlAnchorSubPoint.JunctureType.aligned;

        //Serialization of the list:
        //-> In the serialized context the items and subItems of the list behave like structs that are copied by value, even if they are classes
        //-> The list contains controlPoints and sub-controlPoints with cross references that should be copied by reference. There are constructions how this is possible via using "[SerializeReference]", but serialized lists/arrays/reorderableList seem not to be fully compatible with it. Obscure errors appear, for example when clicking the "choose presets symbol" in the component inspector, then these lists dissolve into nullRefExceptions, even if there aren't any presets available. Also other obscure errors for reorderable list like "list item not found" after deleting control points via custom buttons.
        //-> see also Unitys documenation on Serialization: "Avoid nested, recursive structures where you reference other classes."
        //-> Therefore the cross references of the subItems are implemented via fake properties: They don't "reference each other", but they reference only the list-carrying MonoBehaviour-inherited spline-component here and ask for the position of the "(quasi)referenced" other subItem inside the (quasi)struct. References from other gameobjects (that are bound to controlSubPoints via the "SplineConnection"-component) have to act in the same way: They cannot reference the subPoint where they are bound to "directly", but have to obtain it via the position inside the list-(quasi)struct.
        [SerializeField] public List<InternalDXXL_BezierControlPointTriplet2D> listOfControlPointTriplets = new List<InternalDXXL_BezierControlPointTriplet2D>();

        [SerializeField] Vector3 lastGlobalPositionOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf;
        [SerializeField] Quaternion lastGlobalRotationOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf;
        [SerializeField] Vector3 lastLossyScaleOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf; //known issue: if a parent transform scale is set to 0/0/0 in local draw space, then after setting the scale to a valid value again the spline shape cannot be retrieved and is melted to a single point

        public override void InitializeValues_onceInComponentLifetime()
        {
            TrySetTextToEmptyString();

            customVector2_1_picker_isOutfolded = true;
            source_ofCustomVector2_1 = CustomVector2Source.manualInput;
            customVector2_1_clipboardForManualInput = Vector2.right;
            vectorInterpretation_ofCustomVector2_1 = VectorInterpretation.globalSpace;

            customVector2_2_picker_isOutfolded = true;
            source_ofCustomVector2_2 = CustomVector2Source.manualInput;
            customVector2_2_clipboardForManualInput = Vector2.right;
            vectorInterpretation_ofCustomVector2_2 = VectorInterpretation.globalSpace;

            drawSpace = BezierSplineDrawer.DrawSpace.global;
            CreateFirstTwoControlPointsOfNewlyCreatedSpline();
        }

        void CreateFirstTwoControlPointsOfNewlyCreatedSpline()
        {
            CreateNewControlPoint_atSplineEnd();
            CreateNewControlPoint_atSplineEnd();

            Vector2 initialPosOfSecondControlPoint = listOfControlPointTriplets[1].anchorPoint.GetPos_inUnitsOfGlobalSpace() + 2.0f * Vector2.up - 0.75f * Vector2.right;
            listOfControlPointTriplets[1].anchorPoint.SetPos_inUnitsOfGlobalSpace(initialPosOfSecondControlPoint, true, null);
            listOfControlPointTriplets[1].isHighlighted = false;
        }

        void OnDestroy()
        {
            //"OnDestroy" is sometimes not fired. I don't see the clear reason for this. It is not always tied to the reason which the Unity documentation mentions (which is "OnDestroy() is not called for inactive gameobjects"). In the cases where it is not fired the boundGameobject.connectionComponent-references don't get reverted via "Undo (the spline deletion)".
            TryDeleteAllBoundGameobjectConnectionsInclUndo();
        }

        public override void DrawVisualizedObject()
        {
            TryReApplyLocalDrawSpaceValues();

            bool textHasBeenDrawn = false;
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                InternalDXXL_BezierControlPointTriplet2D currControlPointTriplet = listOfControlPointTriplets[i];
                InternalDXXL_BezierControlPointTriplet2D nextControlPointTriplet = GetNextControlPointTriplet(i, true);
                textHasBeenDrawn = DrawBezierSegmentBetweenTwoControlPointTriplets(textHasBeenDrawn, currControlPointTriplet, nextControlPointTriplet);
            }
            DrawTextIfThereArentAnyControlPoints();
        }

        void TryReApplyLocalDrawSpaceValues()
        {
            bool reApplyLocalDrawSpaceValues = false;
            if (drawSpace == BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject)
            {
                if (false == UtilitiesDXXL_Math.CheckIf_twoVectorsAreExactlyEqual(transform.position, lastGlobalPositionOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf))
                {
                    reApplyLocalDrawSpaceValues = true;
                }

                if (false == UtilitiesDXXL_Math.CheckIf_twoQuaternionsAreExactlyEqual(transform.rotation, lastGlobalRotationOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf))
                {
                    reApplyLocalDrawSpaceValues = true;
                }

                if (false == UtilitiesDXXL_Math.CheckIf_twoVectorsAreExactlyEqual(transform.lossyScale, lastLossyScaleOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf))
                {
                    reApplyLocalDrawSpaceValues = true;
                }
            }

            if (reApplyLocalDrawSpaceValues)
            {
                for (int i = 0; i < listOfControlPointTriplets.Count; i++)
                {
                    listOfControlPointTriplets[i].Save_currentPosConfigInUnitsOfActiveDrawSpace_as_posConfigAfterSpaceChangeInUnitsOfActiveDrawSpace();
                    listOfControlPointTriplets[i].ApplySaved_posConfigAfterSpaceChangeInUnitsOfActiveDrawSpace();
                }
                Save_lastTransformStateOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf();
            }
        }

        bool DrawBezierSegmentBetweenTwoControlPointTriplets(bool textHasBeenDrawn, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentStart, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentEnd)
        {
            if (controlPointTriplet_atSegmentEnd != null)
            {
                if (controlPointTriplet_atSegmentStart.forwardHelperPoint.isUsed == true)
                {
                    if (controlPointTriplet_atSegmentEnd.backwardHelperPoint.isUsed == true)
                    {
                        DrawBasics2D.BezierSegmentCubic(controlPointTriplet_atSegmentStart.anchorPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentEnd.anchorPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentStart.forwardHelperPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentEnd.backwardHelperPoint.GetPos_inUnitsOfGlobalSpace(), color, GetTextForCurrentSegment(textHasBeenDrawn), lineWidth, straightSubDivisionsPerSegment, GetZPos_global_for2D(), false, textSize, 0.0f, hiddenByNearerObjects);
                    }
                    else
                    {
                        DrawBasics2D.BezierSegmentQuadratic(controlPointTriplet_atSegmentStart.anchorPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentEnd.anchorPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentStart.forwardHelperPoint.GetPos_inUnitsOfGlobalSpace(), color, GetTextForCurrentSegment(textHasBeenDrawn), lineWidth, straightSubDivisionsPerSegment, GetZPos_global_for2D(), textSize, 0.0f, hiddenByNearerObjects);
                    }
                }
                else
                {
                    if (controlPointTriplet_atSegmentEnd.backwardHelperPoint.isUsed == true)
                    {
                        DrawBasics2D.BezierSegmentQuadratic(controlPointTriplet_atSegmentStart.anchorPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentEnd.anchorPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentEnd.backwardHelperPoint.GetPos_inUnitsOfGlobalSpace(), color, GetTextForCurrentSegment(textHasBeenDrawn), lineWidth, straightSubDivisionsPerSegment, GetZPos_global_for2D(), textSize, 0.0f, hiddenByNearerObjects);
                    }
                    else
                    {
                        Line_fadeableAnimSpeed_2D.InternalDraw(controlPointTriplet_atSegmentStart.anchorPoint.GetPos_inUnitsOfGlobalSpace(), controlPointTriplet_atSegmentEnd.anchorPoint.GetPos_inUnitsOfGlobalSpace(), color, lineWidth, GetTextForCurrentSegment(textHasBeenDrawn), DrawBasics.LineStyle.solid, GetZPos_global_for2D(), 1.0f, 0.0f, null, 0.0f, 0.0f, 0.0f, 0.0f, hiddenByNearerObjects, false, false);
                    }
                }
            }

            textHasBeenDrawn = true;
            return textHasBeenDrawn;
        }

        string GetTextForCurrentSegment(bool textHasBeenDrawn)
        {
            return textHasBeenDrawn ? null : text_inclGlobalMarkupTags;
        }

        void DrawTextIfThereArentAnyControlPoints()
        {
            Vector2 textPosGlobal = default;
            if (text_inclGlobalMarkupTags != null && text_inclGlobalMarkupTags != "")
            {
                if (listOfControlPointTriplets.Count == 0)
                {
                    textPosGlobal = Get_originPos_ofActiveDrawSpace_inUnitsOfGlobalSpace();
                }

                if ((listOfControlPointTriplets.Count == 1) && (gapFromEndToStart_isClosed == false))
                {
                    textPosGlobal = listOfControlPointTriplets[0].anchorPoint.GetPos_inUnitsOfGlobalSpace();
                }

                if (listOfControlPointTriplets.Count == 0 || ((listOfControlPointTriplets.Count == 1) && (gapFromEndToStart_isClosed == false)))
                {
                    UtilitiesDXXL_Text.Write2DFramed(text_inclGlobalMarkupTags, textPosGlobal, color, textSize, default(Vector2), DrawText.TextAnchorDXXL.LowerLeftOfFirstLine, GetZPos_global_for2D(), DrawBasics.LineStyle.invisible, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, true, 0.0f, hiddenByNearerObjects);
                }
            }
        }

        public void CreateNewControlPoint_dueToPlusButtonBelowControlPointsListHasBeenClicked()
        {
            CreateNewControlPoint_atSplineEnd();
        }

        public void TryDeleteControlPoint_dueToMinusButtonAtControlPointListItemHasBeenClicked(int i_ofItemToDelete)
        {
            RegisterStateForUndo("Delete Spline Point(s)", true, true);
            DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet(i_ofItemToDelete);
            listOfControlPointTriplets.RemoveAt(i_ofItemToDelete);
            ReassignIndexesToAllControlPoints();
            TryDeactivateHelperPointsAtSplineEndsToVoid();
        }

        void TryDeleteAllBoundGameobjectConnectionsInclUndo()
        {
            //-> this ensures the retrieval of the boundGameobject-references if after spline-deletion "Editor/Undo" is used
            if (listOfControlPointTriplets != null)
            {
                for (int i = 0; i < listOfControlPointTriplets.Count; i++)
                {
                    DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet(i);
                }
            }
        }

        void DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet(int i_ofControlPointTripletWhereConnectionsGetDeleted)
        {
            if (Application.isPlaying)
            {
                DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet_nonImmediate(i_ofControlPointTripletWhereConnectionsGetDeleted);
            }
            else
            {
                DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet_immediate(i_ofControlPointTripletWhereConnectionsGetDeleted);
            }
        }

        void DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet_nonImmediate(int i_ofControlPointTripletWhereConnectionsGetDeleted)
        {
            if (listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].backwardHelperPoint.connectionComponent_onBoundGameobject != null)
            {
                Destroy(listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].backwardHelperPoint.connectionComponent_onBoundGameobject);
            }
            if (listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].anchorPoint.connectionComponent_onBoundGameobject != null)
            {
                Destroy(listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].anchorPoint.connectionComponent_onBoundGameobject);
            }
            if (listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].forwardHelperPoint.connectionComponent_onBoundGameobject != null)
            {
                Destroy(listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].forwardHelperPoint.connectionComponent_onBoundGameobject);
            }
        }

        void DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet_immediate(int i_ofControlPointTripletWhereConnectionsGetDeleted)
        {
#if UNITY_EDITOR
            if (listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].backwardHelperPoint.connectionComponent_onBoundGameobject != null)
            {
                UnityEditor.Undo.DestroyObjectImmediate(listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].backwardHelperPoint.connectionComponent_onBoundGameobject);
            }
            if (listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].anchorPoint.connectionComponent_onBoundGameobject != null)
            {
                UnityEditor.Undo.DestroyObjectImmediate(listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].anchorPoint.connectionComponent_onBoundGameobject);
            }
            if (listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].forwardHelperPoint.connectionComponent_onBoundGameobject != null)
            {
                UnityEditor.Undo.DestroyObjectImmediate(listOfControlPointTriplets[i_ofControlPointTripletWhereConnectionsGetDeleted].forwardHelperPoint.connectionComponent_onBoundGameobject);
            }
#else
            DeleteConnectionComponentsOfBoundGameobjects_onWholeControlPointTriplet_nonImmediate(i_ofControlPointTripletWhereConnectionsGetDeleted);
#endif
        }

        public void DeleteConnectionComponentOfBoundGameobject_onControlSubPoint(int i_ofControlPointWhereConnectionsGetDeleted, InternalDXXL_BezierControlSubPoint.SubPointType subPointType_whereConnectionsGetDeleted)
        {
            if (Application.isPlaying)
            {
                DeleteConnectionComponentOfBoundGameobject_onControlSubPoint_nonImmediate(i_ofControlPointWhereConnectionsGetDeleted, subPointType_whereConnectionsGetDeleted);
            }
            else
            {
                DeleteConnectionComponentOfBoundGameobject_onControlSubPoint_immediate(i_ofControlPointWhereConnectionsGetDeleted, subPointType_whereConnectionsGetDeleted);
            }
        }

        public void DeleteConnectionComponentOfBoundGameobject_onControlSubPoint_nonImmediate(int i_ofControlPointWhereConnectionsGetDeleted, InternalDXXL_BezierControlSubPoint.SubPointType subPointType_whereConnectionsGetDeleted)
        {
            if (listOfControlPointTriplets[i_ofControlPointWhereConnectionsGetDeleted].GetASubPoint(subPointType_whereConnectionsGetDeleted).connectionComponent_onBoundGameobject != null)
            {
                Destroy(listOfControlPointTriplets[i_ofControlPointWhereConnectionsGetDeleted].GetASubPoint(subPointType_whereConnectionsGetDeleted).connectionComponent_onBoundGameobject);
            }
        }

        public void DeleteConnectionComponentOfBoundGameobject_onControlSubPoint_immediate(int i_ofControlPointWhereConnectionsGetDeleted, InternalDXXL_BezierControlSubPoint.SubPointType subPointType_whereConnectionsGetDeleted)
        {
#if UNITY_EDITOR
            if (listOfControlPointTriplets[i_ofControlPointWhereConnectionsGetDeleted].GetASubPoint(subPointType_whereConnectionsGetDeleted).connectionComponent_onBoundGameobject != null)
            {
                UnityEditor.Undo.DestroyObjectImmediate(listOfControlPointTriplets[i_ofControlPointWhereConnectionsGetDeleted].GetASubPoint(subPointType_whereConnectionsGetDeleted).connectionComponent_onBoundGameobject);
            }
#else
            DeleteConnectionComponentOfBoundGameobject_onControlSubPoint_nonImmediate(i_ofControlPointWhereConnectionsGetDeleted, subPointType_whereConnectionsGetDeleted);
#endif
        }

        void TryDeactivateHelperPointsAtSplineEndsToVoid()
        {
            if (gapFromEndToStart_isClosed == false)
            {
                if (listOfControlPointTriplets.Count > 0)
                {
                    listOfControlPointTriplets[0].backwardHelperPoint.ChangeUsedState(false, false);
                    listOfControlPointTriplets[0].anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);

                    int i_ofLastControlPoint = listOfControlPointTriplets.Count - 1;
                    listOfControlPointTriplets[i_ofLastControlPoint].forwardHelperPoint.ChangeUsedState(false, false);
                    listOfControlPointTriplets[i_ofLastControlPoint].anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
                }
            }
        }

        public void CreateNewControlPoint_atSplineEnd()
        {
            RegisterStateForUndo("Add Spline Point", false, false);
            InternalDXXL_BezierControlPointTriplet2D newControlPointTriplet = new InternalDXXL_BezierControlPointTriplet2D();
            listOfControlPointTriplets.Add(newControlPointTriplet);
            ReassignIndexesToAllControlPoints();

            if (listOfControlPointTriplets.Count >= 2)
            {
                InitializeNewlyCreatedControlPoint_atSplineEnd();
            }
            else
            {
                InitializeFirstControlPoint();
            }

            SetSelectedListSlot(listOfControlPointTriplets.Count - 1);
        }

        public void CreateNewControlPoint_atSplineStart()
        {
            RegisterStateForUndo("Add Spline Point", false, false);
            InternalDXXL_BezierControlPointTriplet2D newControlPointTriplet = new InternalDXXL_BezierControlPointTriplet2D();
            listOfControlPointTriplets.Insert(0, newControlPointTriplet);
            ReassignIndexesToAllControlPoints();

            if (listOfControlPointTriplets.Count >= 2)
            {
                InitializeNewControlPoint_atSplineStart();
                SetSelectedListSlot(0);
            }
            else
            {
                UtilitiesDXXL_Log.PrintErrorCode("80-" + listOfControlPointTriplets.Count);
            }
        }

        public void CreateNewControlPoint_somewhereOnUpcomingSplineSegment(int i_startOfSegmentPreInsert)
        {
            RegisterStateForUndo("Add Spline Point", false, false);

            InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentStartPreInsert = listOfControlPointTriplets[i_startOfSegmentPreInsert];
            InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentEndPreInsert = controlPointTriplet_atSegmentStartPreInsert.GetNextControlPointTripletAlongSplineDir(true);

            if (controlPointTriplet_atSegmentEndPreInsert != null)
            {
                Vector2 posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentStartPreInsert.GetPosAtPlusButton_onUpcomingBezierSegment_inUnitsOfGlobalSpace();

                if (controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.isUsed == true)
                {
                    if (controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.isUsed == true)
                    {
                        CreateSubdividingControlPoint_insideCubicSegment(i_startOfSegmentPreInsert, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, controlPointTriplet_atSegmentStartPreInsert, controlPointTriplet_atSegmentEndPreInsert);
                    }
                    else
                    {
                        CreateSubdividingControlPoint_insideQuadraticSegmentThatHasOnlyStartPointsForwardHelper(i_startOfSegmentPreInsert, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, controlPointTriplet_atSegmentStartPreInsert, controlPointTriplet_atSegmentEndPreInsert);
                    }
                }
                else
                {
                    if (controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.isUsed == true)
                    {
                        CreateSubdividingControlPoint_insideQuadraticSegmentThatHasOnlyEndPointsBackwardHelper(i_startOfSegmentPreInsert, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, controlPointTriplet_atSegmentStartPreInsert, controlPointTriplet_atSegmentEndPreInsert);
                    }
                    else
                    {
                        CreateSubdividingControlPoint_insideStraightSegmentThatHasNoHelpers(i_startOfSegmentPreInsert, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, controlPointTriplet_atSegmentStartPreInsert, controlPointTriplet_atSegmentEndPreInsert);
                    }
                }

                controlPointTriplet_atSegmentStartPreInsert.progress0to1_ofPlusButtonPosition_inUpcomingBezierSegment = 0.5f;
            }
            else
            {
                UtilitiesDXXL_Log.PrintErrorCode("57-" + i_startOfSegmentPreInsert + "-" + listOfControlPointTriplets.Count + "" + gapFromEndToStart_isClosed);
            }
        }

        void CreateSubdividingControlPoint_insideCubicSegment(int i_startOfSegmentPreInsert, Vector2 posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentStartPreInsert, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentEndPreInsert)
        {
            InternalDXXL_BezierControlPointTriplet2D newlyCreatedControlPointTriplet = InsertUninitializedNewControlPointIntoList(i_startOfSegmentPreInsert);
            Vector2 initialDirection_normalized = Vector2.right; //-> is anyway not used, but immediately overwritten inside this function
            newlyCreatedControlPointTriplet.Initialize(this, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, initialDirection_normalized, InternalDXXL_BezierControlAnchorSubPoint.JunctureType.aligned);

            //all vectors are meant "_inUnitsOfGlobalSpace": 
            float progress0to1_insidePreInsertSegment = controlPointTriplet_atSegmentStartPreInsert.progress0to1_ofPlusButtonPosition_inUpcomingBezierSegment;

            Vector2 from_preInsertStartForwardHelper_to_preInsertEndBackwardHelper = controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.GetPos_inUnitsOfGlobalSpace() - controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.GetPos_inUnitsOfGlobalSpace();
            Vector2 interpolatedPosOnStrech_from_preInsertForwardHelper_to_preInsertEndBackwardHelper = controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.GetPos_inUnitsOfGlobalSpace() + from_preInsertStartForwardHelper_to_preInsertEndBackwardHelper * progress0to1_insidePreInsertSegment;
            Vector2 from_anchorOfStartPointPreInsert_to_forwardHelperOfStartPointPostInsert = (controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.GetPos_inUnitsOfGlobalSpace() - controlPointTriplet_atSegmentStartPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace()) * progress0to1_insidePreInsertSegment;
            Vector2 startPointsForwardHelperPostInsert = controlPointTriplet_atSegmentStartPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace() + from_anchorOfStartPointPreInsert_to_forwardHelperOfStartPointPostInsert;
            Vector2 from_forwardHelperOfStartPointPostInsert_interpolatedPosOnPreInsertStartHelperToEndHelper = interpolatedPosOnStrech_from_preInsertForwardHelper_to_preInsertEndBackwardHelper - startPointsForwardHelperPostInsert;
            Vector2 posOfBackwardHelper_ofNewlyCreatedControlPoint = startPointsForwardHelperPostInsert + from_forwardHelperOfStartPointPostInsert_interpolatedPosOnPreInsertStartHelperToEndHelper * progress0to1_insidePreInsertSegment;
            newlyCreatedControlPointTriplet.backwardHelperPoint.SetPos_inUnitsOfGlobalSpace(posOfBackwardHelper_ofNewlyCreatedControlPoint, true, null);

            float new_absDistanceOfBackwardHelperOfPreInsertEndPoint_ifSegmentWouldNotBeMirrorForced = controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.Get_absDistanceToAnchorPoint_inUnitsOfGlobalSpace() * (1.0f - progress0to1_insidePreInsertSegment);
            Vector2 vector_fromPreInsertSegmentEndAnchor_toPreInsertSegmentEndBackwardHelperAfterInsert_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.Get_direction_fromMountingAnchorToThisHelperPoint_inUnitsOfGlobalSpace_normalized() * new_absDistanceOfBackwardHelperOfPreInsertEndPoint_ifSegmentWouldNotBeMirrorForced;
            Vector2 pos_ofPreInsertSegmentEndBackwardHelperAfterInsert_inUnitsOfGlobalSpace_ifSegmentWouldNotBeMirrorForced = controlPointTriplet_atSegmentEndPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace() + vector_fromPreInsertSegmentEndAnchor_toPreInsertSegmentEndBackwardHelperAfterInsert_inUnitsOfGlobalSpace;
            if (controlPointTriplet_atSegmentEndPreInsert.anchorPoint.junctureType != InternalDXXL_BezierControlAnchorSubPoint.JunctureType.mirrored)
            {
                controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.SetPos_inUnitsOfGlobalSpace(pos_ofPreInsertSegmentEndBackwardHelperAfterInsert_inUnitsOfGlobalSpace_ifSegmentWouldNotBeMirrorForced, true, null);
            }
            else
            {
                LogMessageThatExplainsWhyTheSplineShapeChangedOnPointCreation(false);
            }

            Vector2 from_interpolatedPosOnPreInsertStartHelperToEndHelper_to_endPointsBackwardHelperPostInsert = pos_ofPreInsertSegmentEndBackwardHelperAfterInsert_inUnitsOfGlobalSpace_ifSegmentWouldNotBeMirrorForced - interpolatedPosOnStrech_from_preInsertForwardHelper_to_preInsertEndBackwardHelper;
            Vector2 posOfForwardHelper_ofNewlyCreatedControlPoint = interpolatedPosOnStrech_from_preInsertForwardHelper_to_preInsertEndBackwardHelper + from_interpolatedPosOnPreInsertStartHelperToEndHelper_to_endPointsBackwardHelperPostInsert * progress0to1_insidePreInsertSegment;
            newlyCreatedControlPointTriplet.forwardHelperPoint.SetPos_inUnitsOfGlobalSpace(posOfForwardHelper_ofNewlyCreatedControlPoint, true, null);

            ScaleForwardDistance_ofPreInsertStartPoint(controlPointTriplet_atSegmentStartPreInsert);
            SetDefaultJunctureType_forCase_createInsideCubicSegment(newlyCreatedControlPointTriplet);
        }

        void SetDefaultJunctureType_forCase_createInsideCubicSegment(InternalDXXL_BezierControlPointTriplet2D newlyCreatedControlPointTriplet)
        {
            if (junctureType_ofNewlyCreatedPoints == InternalDXXL_BezierControlAnchorSubPoint.JunctureType.mirrored)
            {
                newlyCreatedControlPointTriplet.anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.mirrored);
                Debug.Log("Spline Position Creation Information: The spline segment before the newly created control point doesn't fit the spline shape from before the point was created. The reason for this is that the default juncture type of newly created control points is 'mirrored'. It is not possible to keep the spline shape with this constraint.");
            }

            if (junctureType_ofNewlyCreatedPoints == InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked)
            {
                newlyCreatedControlPointTriplet.anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
            }
        }

        void CreateSubdividingControlPoint_insideQuadraticSegmentThatHasOnlyStartPointsForwardHelper(int i_startOfSegmentPreInsert, Vector2 posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentStartPreInsert, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentEndPreInsert)
        {
            if (controlPointTriplet_atSegmentEndPreInsert.anchorPoint.junctureType != InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked)
            {
                UtilitiesDXXL_Log.PrintErrorCode("67-" + i_startOfSegmentPreInsert + "-" + listOfControlPointTriplets.Count + "-" + gapFromEndToStart_isClosed + "-" + controlPointTriplet_atSegmentStartPreInsert.anchorPoint.junctureType + "-" + controlPointTriplet_atSegmentEndPreInsert.anchorPoint.junctureType);
                return;
            }

            InternalDXXL_BezierControlPointTriplet2D newlyCreatedControlPointTriplet = InsertUninitializedNewControlPointIntoList(i_startOfSegmentPreInsert);
            Vector2 initialDirection_normalized = Vector2.right; //-> is anyway not used, but immediately overwritten inside this function
            newlyCreatedControlPointTriplet.Initialize(this, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, initialDirection_normalized, junctureType_ofNewlyCreatedPoints);
            newlyCreatedControlPointTriplet.anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
            newlyCreatedControlPointTriplet.backwardHelperPoint.ChangeUsedState(false, false);

            Vector2 from_forwardHelperPosOfPreInsertStartPoint_to_preInsertEndPointAnchor_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentEndPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace() - controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.GetPos_inUnitsOfGlobalSpace();
            Vector2 posOfForwardHelper_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.GetPos_inUnitsOfGlobalSpace() + from_forwardHelperPosOfPreInsertStartPoint_to_preInsertEndPointAnchor_inUnitsOfGlobalSpace * controlPointTriplet_atSegmentStartPreInsert.progress0to1_ofPlusButtonPosition_inUpcomingBezierSegment;
            newlyCreatedControlPointTriplet.forwardHelperPoint.SetPos_inUnitsOfGlobalSpace(posOfForwardHelper_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace, true, null);

            ScaleForwardDistance_ofPreInsertStartPoint(controlPointTriplet_atSegmentStartPreInsert);
            SetDefaultJunctureType_forCase_createInsideQuadraticSegment(newlyCreatedControlPointTriplet, "before", "end");
        }

        void CreateSubdividingControlPoint_insideQuadraticSegmentThatHasOnlyEndPointsBackwardHelper(int i_startOfSegmentPreInsert, Vector2 posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentStartPreInsert, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentEndPreInsert)
        {
            if (controlPointTriplet_atSegmentStartPreInsert.anchorPoint.junctureType != InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked)
            {
                UtilitiesDXXL_Log.PrintErrorCode("68-" + i_startOfSegmentPreInsert + "-" + listOfControlPointTriplets.Count + "-" + gapFromEndToStart_isClosed + "-" + controlPointTriplet_atSegmentStartPreInsert.anchorPoint.junctureType + "-" + controlPointTriplet_atSegmentEndPreInsert.anchorPoint.junctureType);
                return;
            }

            InternalDXXL_BezierControlPointTriplet2D newlyCreatedControlPointTriplet = InsertUninitializedNewControlPointIntoList(i_startOfSegmentPreInsert);
            Vector2 initialDirection_normalized = Vector2.right; //-> is anyway not used, but immediately overwritten inside this function
            newlyCreatedControlPointTriplet.Initialize(this, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, initialDirection_normalized, junctureType_ofNewlyCreatedPoints);
            newlyCreatedControlPointTriplet.anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
            newlyCreatedControlPointTriplet.forwardHelperPoint.ChangeUsedState(false, false);

            Vector2 from_anchorPosOfPreInsertStartPoint_to_preInsertEndPointsBackwardHelper_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.GetPos_inUnitsOfGlobalSpace() - controlPointTriplet_atSegmentStartPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace();
            Vector2 posOfBackwardHelper_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentStartPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace() + from_anchorPosOfPreInsertStartPoint_to_preInsertEndPointsBackwardHelper_inUnitsOfGlobalSpace * controlPointTriplet_atSegmentStartPreInsert.progress0to1_ofPlusButtonPosition_inUpcomingBezierSegment;
            newlyCreatedControlPointTriplet.backwardHelperPoint.SetPos_inUnitsOfGlobalSpace(posOfBackwardHelper_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace, true, null);

            if (controlPointTriplet_atSegmentEndPreInsert.anchorPoint.junctureType != InternalDXXL_BezierControlAnchorSubPoint.JunctureType.mirrored)
            {
                float new_absDistanceOfBackwardHelperOfPreInsertEndPoint_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.Get_absDistanceToAnchorPoint_inUnitsOfGlobalSpace() * (1.0f - controlPointTriplet_atSegmentStartPreInsert.progress0to1_ofPlusButtonPosition_inUpcomingBezierSegment);
                controlPointTriplet_atSegmentEndPreInsert.backwardHelperPoint.Set_absDistanceToAnchorPoint_inUnitsOfGlobalSpace(new_absDistanceOfBackwardHelperOfPreInsertEndPoint_inUnitsOfGlobalSpace, true, null);
            }
            else
            {
                LogMessageThatExplainsWhyTheSplineShapeChangedOnPointCreation(false);
            }

            SetDefaultJunctureType_forCase_createInsideQuadraticSegment(newlyCreatedControlPointTriplet, "after", "start");
        }

        void SetDefaultJunctureType_forCase_createInsideQuadraticSegment(InternalDXXL_BezierControlPointTriplet2D newlyCreatedControlPointTriplet, string segment_identifier, string disabledWeightPoint_identifier)
        {
            if (junctureType_ofNewlyCreatedPoints == InternalDXXL_BezierControlAnchorSubPoint.JunctureType.aligned)
            {
                newlyCreatedControlPointTriplet.anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.aligned);
                LogInfoThatSplineShapeChangedOnPointCreation_dueToDefaultJunctureTypeDoesntFitQuadraticSegment(segment_identifier, disabledWeightPoint_identifier, "an");
            }

            if (junctureType_ofNewlyCreatedPoints == InternalDXXL_BezierControlAnchorSubPoint.JunctureType.mirrored)
            {
                newlyCreatedControlPointTriplet.anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.mirrored);
                LogInfoThatSplineShapeChangedOnPointCreation_dueToDefaultJunctureTypeDoesntFitQuadraticSegment(segment_identifier, disabledWeightPoint_identifier, "a");
            }
        }

        void LogInfoThatSplineShapeChangedOnPointCreation_dueToDefaultJunctureTypeDoesntFitQuadraticSegment(string segment_identifier, string disabledWeightPoint_identifier, string indefiniteArticle_ofDefaultJunctureType)
        {
            Debug.Log("Spline Position Creation Information: The spline segment " + segment_identifier + " the newly created control point doesn't fit the spline shape from before the point was created. The reason for this is that the default juncture type of newly created control points is '" + junctureType_ofNewlyCreatedPoints + "', but the weight point at the " + disabledWeightPoint_identifier + " of the pre-insert segment is disabled. It is not possible to keep the spline shape with " + indefiniteArticle_ofDefaultJunctureType + " " + junctureType_ofNewlyCreatedPoints + " juncture type if not both weight points of the pre-insert segment are enabled.");
        }

        void ScaleForwardDistance_ofPreInsertStartPoint(InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentStartPreInsert)
        {
            if (controlPointTriplet_atSegmentStartPreInsert.anchorPoint.junctureType != InternalDXXL_BezierControlAnchorSubPoint.JunctureType.mirrored)
            {
                float new_absDistanceOfForwardHelperOfPreInsertStartPoint_inUnitsOfGlobalSpace = controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.Get_absDistanceToAnchorPoint_inUnitsOfGlobalSpace() * controlPointTriplet_atSegmentStartPreInsert.progress0to1_ofPlusButtonPosition_inUpcomingBezierSegment;
                controlPointTriplet_atSegmentStartPreInsert.forwardHelperPoint.Set_absDistanceToAnchorPoint_inUnitsOfGlobalSpace(new_absDistanceOfForwardHelperOfPreInsertStartPoint_inUnitsOfGlobalSpace, true, null);
            }
            else
            {
                LogMessageThatExplainsWhyTheSplineShapeChangedOnPointCreation(true);
            }
        }

        void LogMessageThatExplainsWhyTheSplineShapeChangedOnPointCreation(bool theMessageCorrespondsTo_theSplinePartAtPreInsertSTARTpoint_notAtPreInsertENDpoint)
        {
            string segment_identifier; //-> this is actually only specified to prevent the confusion that the same message could be thrown twice for a single point creation
            if (theMessageCorrespondsTo_theSplinePartAtPreInsertSTARTpoint_notAtPreInsertENDpoint)
            {
                segment_identifier = "before";
            }
            else
            {
                segment_identifier = "after";
            }
            Debug.Log("Spline Position Creation Information: The spline segment " + segment_identifier + " the newly created control point doesn't fit the spline shape from before the point was created. The reason for this is that the neighboring control point has a 'mirrored' juncture type. Otherwise the neighboring segment would have changed it's shape.");
        }

        void CreateSubdividingControlPoint_insideStraightSegmentThatHasNoHelpers(int i_startOfSegmentPreInsert, Vector2 posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentStartPreInsert, InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_atSegmentEndPreInsert)
        {
            InternalDXXL_BezierControlPointTriplet2D newlyCreatedControlPointTriplet = InsertUninitializedNewControlPointIntoList(i_startOfSegmentPreInsert);
            Vector2 initialDirection_normalized = Vector2.right; //-> is anyway not used, but immediately overwritten inside this function
            newlyCreatedControlPointTriplet.Initialize(this, posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, initialDirection_normalized, junctureType_ofNewlyCreatedPoints);

            Vector2 initialPosOfForwardHelper_inUnitsOfGlobalSpace = UtilitiesDXXL_Math.GetCenterBetweenTwoPoints(posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, controlPointTriplet_atSegmentEndPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace());
            newlyCreatedControlPointTriplet.forwardHelperPoint.SetPos_inUnitsOfGlobalSpace(initialPosOfForwardHelper_inUnitsOfGlobalSpace, true, null);

            Vector2 initialPosOfBackwardHelper_inUnitsOfGlobalSpace = UtilitiesDXXL_Math.GetCenterBetweenTwoPoints(posOfNewlyCreatedControlPoint_inUnitsOfGlobalSpace, controlPointTriplet_atSegmentStartPreInsert.anchorPoint.GetPos_inUnitsOfGlobalSpace());
            newlyCreatedControlPointTriplet.backwardHelperPoint.SetPos_inUnitsOfGlobalSpace(initialPosOfBackwardHelper_inUnitsOfGlobalSpace, true, null);
        }

        InternalDXXL_BezierControlPointTriplet2D InsertUninitializedNewControlPointIntoList(int i_startOfSegmentPreInsert)
        {
            InternalDXXL_BezierControlPointTriplet2D newControlPointTriplet = new InternalDXXL_BezierControlPointTriplet2D();
            int i_insertionSlot = i_startOfSegmentPreInsert + 1;
            listOfControlPointTriplets.Insert(i_insertionSlot, newControlPointTriplet);
            ReassignIndexesToAllControlPoints();
            return newControlPointTriplet;
        }

        public void ReassignIndexesToAllControlPoints()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].ReassignIndexInsideControlPointsList(i);
            }
        }

        void InitializeFirstControlPoint()
        {
            Vector2 initialPos_inUnitsOfGlobalSpace = Get_originPos_ofActiveDrawSpace_inUnitsOfGlobalSpace();
            Vector2 initialForwardDir_inUnitsOfGlobalSpace_normalized = Get_right_ofActiveDrawSpace_inUnitsOfGlobalSpace_normalized();

            listOfControlPointTriplets[0].Initialize(this, initialPos_inUnitsOfGlobalSpace, initialForwardDir_inUnitsOfGlobalSpace_normalized, junctureType_ofNewlyCreatedPoints);

            if (gapFromEndToStart_isClosed == false)
            {
                listOfControlPointTriplets[0].backwardHelperPoint.ChangeUsedState(false, false);
                listOfControlPointTriplets[0].forwardHelperPoint.ChangeUsedState(false, false);
                listOfControlPointTriplets[0].anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
            }
        }

        void InitializeNewlyCreatedControlPoint_atSplineEnd()
        {
            int i_ofNewControlPoint = listOfControlPointTriplets.Count - 1; //"i_ofNewControlPoint" is guaranteed bigger than 0 here, so the controlPoint list has at least 2 items
            InternalDXXL_BezierControlPointTriplet2D previouslyLastControlPointTriplet = GetPreviousControlPointTriplet(i_ofNewControlPoint, false);
            Vector2 previouslyLastControlPoint_to_newlyCreatedControlPointAtSplineEnd_inUnitsOfGlobalSpace_normalized = Get_previouslyLastControlPoint_to_newlyCreatedControlPointAtSplineEnd_inUnitsOfGlobalSpace_normalized(previouslyLastControlPointTriplet);
            float distance_from_prevLastControlPoint_to_newlyCreatedControlPoint_inUnitsOfGlobalSpace = Get_distance_from_prevLastControlPoint_to_newlyCreatedControlPoint_inUnitsOfGlobalSpace();
            Vector2 initialPos_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace = previouslyLastControlPointTriplet.anchorPoint.GetPos_inUnitsOfGlobalSpace() + previouslyLastControlPoint_to_newlyCreatedControlPointAtSplineEnd_inUnitsOfGlobalSpace_normalized * distance_from_prevLastControlPoint_to_newlyCreatedControlPoint_inUnitsOfGlobalSpace;
            Vector2 initialForwardDir_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace_normalized = Get_initialFowardDir_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace_normalized(previouslyLastControlPoint_to_newlyCreatedControlPointAtSplineEnd_inUnitsOfGlobalSpace_normalized);

            listOfControlPointTriplets[i_ofNewControlPoint].Initialize(this, initialPos_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace, initialForwardDir_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace_normalized, junctureType_ofNewlyCreatedPoints);

            if (gapFromEndToStart_isClosed == false)
            {
                listOfControlPointTriplets[i_ofNewControlPoint].anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
                listOfControlPointTriplets[i_ofNewControlPoint].forwardHelperPoint.ChangeUsedState(false, false);

                int i_ofPreviouslyLastControlPoint = i_ofNewControlPoint - 1;
                if (listOfControlPointTriplets[i_ofPreviouslyLastControlPoint].IsEndPointToVoid_atStartOrEndOfAnUnclosedSpline() == false)
                {
                    listOfControlPointTriplets[i_ofPreviouslyLastControlPoint].anchorPoint.SetJunctureType(junctureType_ofNewlyCreatedPoints);
                }
                listOfControlPointTriplets[i_ofPreviouslyLastControlPoint].forwardHelperPoint.ChangeUsedState(true, false); //-> this is for the case when "junctureType_ofNewlyCreatedPoints == kinked". Then "SetJunctureType" doesn't do anything (since the new junctureType doesn't differ from the previous one) and therefore also didn't activate the formerly unused forwardHelper
            }
        }

        void InitializeNewControlPoint_atSplineStart()
        {
            //"listOfControlPointRefs.Count" is guaranteed bigger than 0 here, so the controlPoint list has at least 2 items
            InternalDXXL_BezierControlPointTriplet2D previouslyFirstControlPointTriplet = listOfControlPointTriplets[1];
            Vector2 previouslyFirstControlPoint_to_newlyCreatedControlPointAtSplineStart_inUnitsOfGlobalSpace_normalized = Get_firstControlPoint_to_newlyCreatedControlPointAtSplineStart_inUnitsOfGlobalSpace_normalized(previouslyFirstControlPointTriplet);
            float distance_from_prevFirstControlPoint_to_newlyCreatedControlPoint_inUnitsOfGlobalSpace = Get_distance_from_prevLastControlPoint_to_newlyCreatedControlPoint_inUnitsOfGlobalSpace();
            Vector2 initialPos_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace = previouslyFirstControlPointTriplet.anchorPoint.GetPos_inUnitsOfGlobalSpace() + previouslyFirstControlPoint_to_newlyCreatedControlPointAtSplineStart_inUnitsOfGlobalSpace_normalized * distance_from_prevFirstControlPoint_to_newlyCreatedControlPoint_inUnitsOfGlobalSpace;
            Vector2 initialForwardDir_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace_normalized = Get_initialFowardDir_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace_normalized(-previouslyFirstControlPoint_to_newlyCreatedControlPointAtSplineStart_inUnitsOfGlobalSpace_normalized);

            listOfControlPointTriplets[0].Initialize(this, initialPos_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace, initialForwardDir_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace_normalized, junctureType_ofNewlyCreatedPoints);

            if (gapFromEndToStart_isClosed == false)
            {
                listOfControlPointTriplets[0].anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
                listOfControlPointTriplets[0].backwardHelperPoint.ChangeUsedState(false, false);

                int i_ofPreviouslyFirstControlPoint = 1;
                if (listOfControlPointTriplets[i_ofPreviouslyFirstControlPoint].IsEndPointToVoid_atStartOrEndOfAnUnclosedSpline() == false)
                {
                    listOfControlPointTriplets[i_ofPreviouslyFirstControlPoint].anchorPoint.SetJunctureType(junctureType_ofNewlyCreatedPoints);
                }
                listOfControlPointTriplets[i_ofPreviouslyFirstControlPoint].backwardHelperPoint.ChangeUsedState(true, false); //-> this is for the case when "junctureType_ofNewlyCreatedPoints == kinked". Then "SetJunctureType" doesn't do anything (since the new junctureType doesn't differ from the previous one) and therefore also didn't activate the formerly unused backwardHelper
            }
        }

        public Vector2 Get_previouslyLastControlPoint_to_newlyCreatedControlPointAtSplineEnd_inUnitsOfGlobalSpace_normalized(InternalDXXL_BezierControlPointTriplet2D previouslyLastControlPointTriplet)
        {
            //Caller has to take into account: This function may return the zero vector
            switch (definitionType_ofDefaultPosOffset)
            {
                case BezierSplineDrawer.DefinitionType_ofDefaultPosOffset.straightExtentionOfCurveEnd:
                    return Get_forwardTangent_ofControlPointThatDoesntKnowOfANextOne_inUnitsOfGlobalSpace_normalized(previouslyLastControlPointTriplet);
                case BezierSplineDrawer.DefinitionType_ofDefaultPosOffset.customOffset:
                    Vector2 posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace = Get_posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace();
                    return UtilitiesDXXL_Math.GetNormalized_afterScalingIntoRegionOfFloatPrecicion(posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace);
                default:
                    return Vector2.right;
            }
        }

        public Vector2 Get_forwardTangent_ofControlPointThatDoesntKnowOfANextOne_inUnitsOfGlobalSpace_normalized(InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_thatDoesntKnowOfANextOne)
        {
            if (controlPointTriplet_thatDoesntKnowOfANextOne.backwardHelperPoint.isUsed)
            {
                return (-controlPointTriplet_thatDoesntKnowOfANextOne.anchorPoint.Get_direction_toBackward_inUnitsOfGlobalSpace_normalized());
            }
            else
            {
                InternalDXXL_BezierControlSubPoint2D previousUsedNonSuperimposedSubPoint = controlPointTriplet_thatDoesntKnowOfANextOne.backwardHelperPoint.GetPreviousUsedNonSuperimposedSubPointAlongSplineDir(false);
                if (previousUsedNonSuperimposedSubPoint != null)
                {
                    Vector2 previousUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace = controlPointTriplet_thatDoesntKnowOfANextOne.anchorPoint.GetPos_inUnitsOfGlobalSpace() - previousUsedNonSuperimposedSubPoint.GetPos_inUnitsOfGlobalSpace();
                    Vector2 previousUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace_normalized = UtilitiesDXXL_Math.GetNormalized_afterScalingIntoRegionOfFloatPrecicion(previousUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace);
                    if (UtilitiesDXXL_Math.CheckIfNormalizationFailed_meaningLineStayedTooShort(previousUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace_normalized))
                    {
                        return Get_right_ofActiveDrawSpace_inUnitsOfGlobalSpace_normalized();
                    }
                    else
                    {
                        return previousUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace_normalized;
                    }
                }
                else
                {
                    return Get_right_ofActiveDrawSpace_inUnitsOfGlobalSpace_normalized();
                }
            }
        }

        public Vector2 Get_firstControlPoint_to_newlyCreatedControlPointAtSplineStart_inUnitsOfGlobalSpace_normalized(InternalDXXL_BezierControlPointTriplet2D previouslyFirstControlPointTriplet)
        {
            //Caller has to take into account: This function may return the zero vector
            switch (definitionType_ofDefaultPosOffset)
            {
                case BezierSplineDrawer.DefinitionType_ofDefaultPosOffset.straightExtentionOfCurveEnd:
                    return Get_backwardTangent_ofControlPointThatDoesntKnowOfAPreviousOne_inUnitsOfGlobalSpace_normalized(previouslyFirstControlPointTriplet);
                case BezierSplineDrawer.DefinitionType_ofDefaultPosOffset.customOffset:
                    Vector2 posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace = Get_posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace();
                    return (-UtilitiesDXXL_Math.GetNormalized_afterScalingIntoRegionOfFloatPrecicion(posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace));
                default:
                    return Vector2.right;
            }
        }

        public Vector2 Get_backwardTangent_ofControlPointThatDoesntKnowOfAPreviousOne_inUnitsOfGlobalSpace_normalized(InternalDXXL_BezierControlPointTriplet2D controlPointTriplet_thatDoesntKnowOfAPreviousOne)
        {
            if (controlPointTriplet_thatDoesntKnowOfAPreviousOne.forwardHelperPoint.isUsed)
            {
                return (-controlPointTriplet_thatDoesntKnowOfAPreviousOne.anchorPoint.Get_direction_toForward_inUnitsOfGlobalSpace_normalized());
            }
            else
            {
                InternalDXXL_BezierControlSubPoint2D nextUsedNonSuperimposedSubPoint = controlPointTriplet_thatDoesntKnowOfAPreviousOne.forwardHelperPoint.GetNextUsedNonSuperimposedSubPointAlongSplineDir(false);
                if (nextUsedNonSuperimposedSubPoint != null)
                {
                    Vector2 nextUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace = controlPointTriplet_thatDoesntKnowOfAPreviousOne.anchorPoint.GetPos_inUnitsOfGlobalSpace() - nextUsedNonSuperimposedSubPoint.GetPos_inUnitsOfGlobalSpace();
                    Vector2 nextUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace_normalized = UtilitiesDXXL_Math.GetNormalized_afterScalingIntoRegionOfFloatPrecicion(nextUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace);
                    if (UtilitiesDXXL_Math.CheckIfNormalizationFailed_meaningLineStayedTooShort(nextUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace_normalized))
                    {
                        return (-Get_right_ofActiveDrawSpace_inUnitsOfGlobalSpace_normalized());
                    }
                    else
                    {
                        return nextUsedNonSuperimposedSubPoint_to_requestingControlPointsAnchor_inUnitsOfGlobalSpace_normalized;
                    }
                }
                else
                {
                    return (-Get_right_ofActiveDrawSpace_inUnitsOfGlobalSpace_normalized());
                }
            }
        }

        public float Get_distance_from_prevLastControlPoint_to_newlyCreatedControlPoint_inUnitsOfGlobalSpace()
        {
            switch (definitionType_ofDefaultPosOffset)
            {
                case BezierSplineDrawer.DefinitionType_ofDefaultPosOffset.straightExtentionOfCurveEnd:
                    return TransformLength_fromUnitsOfActiveDrawSpace_toGlobalSpace(distanceBetweenTriplets_forNewlyCreatedTriplets_caseOf_straightExtentionOfCurveEnd_inUnitsOfActiveDrawSpace);
                case BezierSplineDrawer.DefinitionType_ofDefaultPosOffset.customOffset:
                    return Get_posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace().magnitude;
                default:
                    return 1.0f;
            }
        }

        Vector2 Get_initialFowardDir_ofNewlyCreatedControlPoint_inUnitsOfGlobalSpace_normalized(Vector2 forwardDirThatRepresents_asCurveEnd_inUnitsOfGlobalSpace_normalized)
        {
            //"forwardDirThatRepresents_asCurveEnd_inUnitsOfGlobalSpace_normalized" may be zero vector here.
            switch (definitionType_ofDefaultRot)
            {
                case BezierSplineDrawer.DefinitionType_ofDefaultRot.sameAsCurveEnd:
                    return ReturnGivenVector_orForTooShortVectorsFallbackToActiveDrawSpaceForward(forwardDirThatRepresents_asCurveEnd_inUnitsOfGlobalSpace_normalized);
                case BezierSplineDrawer.DefinitionType_ofDefaultRot.customOrientation:
                    Vector2 forwardDirOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace = Get_orientationOfNewlyCreatedPointAsForwardDirection_atSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace();
                    Vector2 forwardDirOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace_normalized = UtilitiesDXXL_Math.GetNormalized_afterScalingIntoRegionOfFloatPrecicion(forwardDirOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace);
                    return ReturnGivenVector_orForTooShortVectorsFallbackToActiveDrawSpaceForward(forwardDirOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace_normalized);
                default:
                    return Vector2.right;
            }
        }

        Vector2 ReturnGivenVector_orForTooShortVectorsFallbackToActiveDrawSpaceForward(Vector2 givenVector_normalizedOrTooShort)
        {
            if (UtilitiesDXXL_Math.CheckIfNormalizationFailed_meaningLineStayedTooShort(givenVector_normalizedOrTooShort))
            {
                return Get_right_ofActiveDrawSpace_inUnitsOfGlobalSpace_normalized();
            }
            else
            {
                return givenVector_normalizedOrTooShort;
            }
        }

        public void SetSelectedListSlot(int i_toSelect)
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].isHighlighted = (i == i_toSelect);
            }
        }

        public int Get_i_ofFirstHighlightedControlPoint()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                if (listOfControlPointTriplets[i].isHighlighted)
                {
                    return i;
                }
            }
            return (-1);
        }

        public int GetNumberOfHighlightedControlPoints()
        {
            int number = 0;
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                if (listOfControlPointTriplets[i].isHighlighted)
                {
                    number++;
                }
            }
            return number;
        }

        public bool IsFirstControlPoint(int i)
        {
            return (i == 0);
        }

        public bool IsLastControlPoint(int i)
        {
            return (i == (listOfControlPointTriplets.Count - 1));
        }

        public bool IsFirstControlPoint(InternalDXXL_BezierControlPointTriplet2D controlPoint_toCheck)
        {
            if (listOfControlPointTriplets.Count > 0)
            {
                return (listOfControlPointTriplets[0] == controlPoint_toCheck);
            }
            else
            {
                return false;
            }
        }

        public bool IsLastControlPoint(InternalDXXL_BezierControlPointTriplet2D controlPoint_toCheck)
        {
            if (listOfControlPointTriplets.Count > 0)
            {
                return (listOfControlPointTriplets[listOfControlPointTriplets.Count - 1] == controlPoint_toCheck);
            }
            else
            {
                return false;
            }
        }

        public InternalDXXL_BezierControlPointTriplet2D GetNextControlPointTriplet(int i_ofRequestingControlPoint, bool allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet)
        {
            if (gapFromEndToStart_isClosed)
            {
                if (listOfControlPointTriplets.Count == 0)
                {
                    return null;
                }
                else
                {
                    if (listOfControlPointTriplets.Count == 1)
                    {
                        if (allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet)
                        {
                            if (i_ofRequestingControlPoint != 0)
                            {
                                UtilitiesDXXL_Log.PrintErrorCode("71-" + i_ofRequestingControlPoint);
                            }
                            return listOfControlPointTriplets[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        int i_ofNextControlPoint = UtilitiesDXXL_Math.LoopOvershootingIndexIntoCollectionSize(i_ofRequestingControlPoint + 1, listOfControlPointTriplets.Count);
                        return listOfControlPointTriplets[i_ofNextControlPoint];
                    }
                }
            }
            else
            {
                if (IsLastControlPoint(i_ofRequestingControlPoint))
                {
                    return null;
                }
                else
                {
                    return listOfControlPointTriplets[i_ofRequestingControlPoint + 1];
                }
            }
        }

        public InternalDXXL_BezierControlPointTriplet2D GetPreviousControlPointTriplet(int i_ofRequestingControlPoint, bool allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet)
        {
            if (gapFromEndToStart_isClosed)
            {
                if (listOfControlPointTriplets.Count == 0)
                {
                    return null;
                }
                else
                {
                    if (listOfControlPointTriplets.Count == 1)
                    {
                        if (allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet)
                        {
                            if (i_ofRequestingControlPoint != 0)
                            {
                                UtilitiesDXXL_Log.PrintErrorCode("72-" + i_ofRequestingControlPoint);
                            }
                            return listOfControlPointTriplets[0];
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        int i_ofPreviousControlPoint = UtilitiesDXXL_Math.LoopOvershootingIndexIntoCollectionSize(i_ofRequestingControlPoint - 1, listOfControlPointTriplets.Count);
                        return listOfControlPointTriplets[i_ofPreviousControlPoint];
                    }
                }
            }
            else
            {
                if (IsFirstControlPoint(i_ofRequestingControlPoint))
                {
                    return null;
                }
                else
                {
                    return listOfControlPointTriplets[i_ofRequestingControlPoint - 1];
                }
            }
        }

        public InternalDXXL_BezierControlPointTriplet2D GetNextControlPointTriplet(InternalDXXL_BezierControlPointTriplet2D controlPoint_forWhichToGetTheNextNeighbor, bool allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet)
        {
            return GetNextControlPointTriplet(controlPoint_forWhichToGetTheNextNeighbor.i_ofThisPoint_insideControlPointsList, allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet);
        }

        public InternalDXXL_BezierControlPointTriplet2D GetPreviousControlPointTriplet(InternalDXXL_BezierControlPointTriplet2D controlPoint_forWhichToGetThePreviousNeighbor, bool allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet)
        {
            return GetPreviousControlPointTriplet(controlPoint_forWhichToGetThePreviousNeighbor.i_ofThisPoint_insideControlPointsList, allowRequestingControlPointAsResult_inCaseOfRingSplineOfOnlyOneControlPointTriplet);
        }

        Vector2 Get_posOffsetOfNewlyCreatedPointAtSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace()
        {
            return Get_customVector2_1_inGlobalSpaceUnits();
        }

        Vector2 Get_orientationOfNewlyCreatedPointAsForwardDirection_atSplineEnd_case_definedViaCustomVector_inUnitsOfGlobalSpace()
        {
            return Get_customVector2_2_inGlobalSpaceUnits();
        }

        public Vector2 Get_right_ofActiveDrawSpace_inUnitsOfGlobalSpace_normalized()
        {
            switch (drawSpace)
            {
                case BezierSplineDrawer.DrawSpace.global:
                    return Vector2.right;
                case BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject:
                    Vector2 transformRight_asV2 = transform.right;
                    Vector2 transformRight_asV2_normalized = UtilitiesDXXL_Math.GetNormalized_afterScalingIntoRegionOfFloatPrecicion(transformRight_asV2);
                    if (UtilitiesDXXL_Math.CheckIfNormalizationFailed_meaningLineStayedTooShort(transformRight_asV2_normalized))
                    {
                        return Vector2.right;
                    }
                    else
                    {
                        return transformRight_asV2_normalized;
                    }
                default:
                    return Vector2.right;
            }
        }

        public Vector3 Get_originPos_ofActiveDrawSpace_inUnitsOfGlobalSpace()
        {
            switch (drawSpace)
            {
                case BezierSplineDrawer.DrawSpace.global:
                    return new Vector3(0.0f, 0.0f, GetZPos_global_for2D());
                case BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject:
                    return new Vector3(transform.position.x, transform.position.y, GetZPos_global_for2D());
                default:
                    return new Vector3(0.0f, 0.0f, GetZPos_global_for2D());
            }
        }

        public void ChangeDrawSpace(BezierSplineDrawer.DrawSpace newDrawSpace)
        {
            if (newDrawSpace != drawSpace)
            {
                if (keepWorldPos_duringDrawSpaceChange)
                {
                    //same for both draw space change directions (i.e. "to local space" and "to global space"):
                    ConvertSplineShape_onDrawSpaceChange_butKeepWorldPos(newDrawSpace);
                }
                else
                {
                    switch (newDrawSpace)
                    {
                        case BezierSplineDrawer.DrawSpace.global:
                            ConvertSplineShape_fromOldLocalDrawSpace_to_newGlobalDrawSpace();
                            break;
                        case BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject:
                            ConvertSplineShape_fromOldGlobalDrawSpace_to_newLocalDrawSpace();
                            Save_lastTransformStateOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf();
                            break;
                        default:
                            break;
                    }
                }
                SheduleSceneViewRepaint();
            }
        }

        void ConvertSplineShape_onDrawSpaceChange_butKeepWorldPos(BezierSplineDrawer.DrawSpace newDrawSpace)
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].Save_currentPosConfigInUnitsOfGlobalSpace_as_posConfigAfterSpaceChangeInUnitsOfGlobalSpace();
            }

            drawSpace = newDrawSpace;

            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].ApplySaved_posConfigAfterSpaceChangeInUnitsOfGlobalSpace();
            }
        }

        void ConvertSplineShape_fromOldLocalDrawSpace_to_newGlobalDrawSpace()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].Save_currentPosConfigInUnitsOfActiveDrawSpace_as_posConfigAfterSpaceChangeInUnitsOfGlobalSpace();
            }

            drawSpace = BezierSplineDrawer.DrawSpace.global;

            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].ApplySaved_posConfigAfterSpaceChangeInUnitsOfGlobalSpace();
            }
        }

        void ConvertSplineShape_fromOldGlobalDrawSpace_to_newLocalDrawSpace()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].Save_currentPosConfigInUnitsOfGlobalSpace_as_posConfigAfterSpaceChangeInUnitsOfActiveDrawSpace();
            }

            drawSpace = BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject;

            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].ApplySaved_posConfigAfterSpaceChangeInUnitsOfActiveDrawSpace();
            }
        }

        void Save_lastTransformStateOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf()
        {
            lastGlobalPositionOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf = transform.position;
            lastGlobalRotationOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf = transform.rotation;
            lastLossyScaleOfTheSplineComponentHostingGameobject_thatTheLocalDrawSpaceSplineKnowsOf = transform.lossyScale;
        }

        public void ChangeCloseGapState(bool newStateOf_gapIsClosed)
        {
            RegisterStateForUndo("Spline Ring State", false, false);

            gapFromEndToStart_isClosed = newStateOf_gapIsClosed;
            int i_lastControlPoint = listOfControlPointTriplets.Count - 1;

            if (newStateOf_gapIsClosed == true)
            {
                //change from "unclosed" to "closed": 
                listOfControlPointTriplets[0].anchorPoint.SetJunctureType(junctureType_ofNewlyCreatedPoints);
                listOfControlPointTriplets[i_lastControlPoint].anchorPoint.SetJunctureType(junctureType_ofNewlyCreatedPoints);
            }
            else
            {
                //change from "closed" to "unclosed": 
                listOfControlPointTriplets[0].anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
                listOfControlPointTriplets[i_lastControlPoint].anchorPoint.SetJunctureType(InternalDXXL_BezierControlAnchorSubPoint.JunctureType.kinked);
            }

            listOfControlPointTriplets[0].backwardHelperPoint.ChangeUsedState(newStateOf_gapIsClosed, false);
            listOfControlPointTriplets[i_lastControlPoint].forwardHelperPoint.ChangeUsedState(newStateOf_gapIsClosed, false);

            SheduleSceneViewRepaint();
        }

        public bool CheckIf_allFoldableHelperPoints_areUnfolded_inTheInspectorList()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                if (listOfControlPointTriplets[i].CheckIf_foldableHelperPoints_areUnfolded_inTheInspectorList() == false)
                {
                    return false;
                }
            }
            return true;
        }

        public bool CheckIf_allFoldableHelperPoints_areCollapsed_inTheInspectorList()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                if (listOfControlPointTriplets[i].CheckIf_foldableHelperPoints_areCollapsed_inTheInspectorList() == false)
                {
                    return false;
                }
            }
            return true;
        }

        public void UnfoldAllHelperPointInTheInspectorList()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].UnfoldBothHelperPointInTheInspectorList();
            }
        }

        public void CollapseAllHelperPointInTheInspectorList()
        {
            for (int i = 0; i < listOfControlPointTriplets.Count; i++)
            {
                listOfControlPointTriplets[i].CollapseBothHelperPointInTheInspectorList();
            }
        }

        public bool CheckIf_gameobjectToAssign_isAlreadyAssignedAtAnotherSubPointOfTheSpline(GameObject gameobjectToAssign, out int i_ofTripletThatContainsTheSubPointThatAlreadyHasTheGameobjectAssigned, out InternalDXXL_BezierControlSubPoint.SubPointType subPointThatAlreadyHasTheGameobjectAssigned)
        {
            i_ofTripletThatContainsTheSubPointThatAlreadyHasTheGameobjectAssigned = -1; //not further used
            subPointThatAlreadyHasTheGameobjectAssigned = InternalDXXL_BezierControlSubPoint.SubPointType.anchor; //not further used
            if (gameobjectToAssign == null)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < listOfControlPointTriplets.Count; i++)
                {
                    i_ofTripletThatContainsTheSubPointThatAlreadyHasTheGameobjectAssigned = i;

                    if (listOfControlPointTriplets[i].backwardHelperPoint.boundGameobject == gameobjectToAssign)
                    {
                        subPointThatAlreadyHasTheGameobjectAssigned = InternalDXXL_BezierControlSubPoint.SubPointType.backwardHelper;
                        return true;
                    }

                    if (listOfControlPointTriplets[i].anchorPoint.boundGameobject == gameobjectToAssign)
                    {
                        subPointThatAlreadyHasTheGameobjectAssigned = InternalDXXL_BezierControlSubPoint.SubPointType.anchor;
                        return true;
                    }

                    if (listOfControlPointTriplets[i].forwardHelperPoint.boundGameobject == gameobjectToAssign)
                    {
                        subPointThatAlreadyHasTheGameobjectAssigned = InternalDXXL_BezierControlSubPoint.SubPointType.forwardHelper;
                        return true;
                    }
                }
            }
            return false;
        }

        public void RegisterStateForUndo(string nameOfUndoEntry, bool includeTransformsOfAllBoundGameobjects, bool includeConnectionComponentsOfAllBoundGameobjects)
        {
#if UNITY_EDITOR
            UnityEditor.Undo.RegisterCompleteObjectUndo(this, nameOfUndoEntry);

            if (includeTransformsOfAllBoundGameobjects)
            {
                for (int i = 0; i < listOfControlPointTriplets.Count; i++)
                {
                    if (listOfControlPointTriplets[i].backwardHelperPoint.boundGameobject != null)
                    {
                        UnityEditor.Undo.RegisterCompleteObjectUndo(listOfControlPointTriplets[i].backwardHelperPoint.boundGameobject.transform, nameOfUndoEntry);
                    }

                    if (listOfControlPointTriplets[i].anchorPoint.boundGameobject != null)
                    {
                        UnityEditor.Undo.RegisterCompleteObjectUndo(listOfControlPointTriplets[i].anchorPoint.boundGameobject.transform, nameOfUndoEntry);
                    }

                    if (listOfControlPointTriplets[i].forwardHelperPoint.boundGameobject != null)
                    {
                        UnityEditor.Undo.RegisterCompleteObjectUndo(listOfControlPointTriplets[i].forwardHelperPoint.boundGameobject.transform, nameOfUndoEntry);
                    }
                }
            }

            if (includeConnectionComponentsOfAllBoundGameobjects)
            {
                for (int i = 0; i < listOfControlPointTriplets.Count; i++)
                {
                    if (listOfControlPointTriplets[i].backwardHelperPoint.connectionComponent_onBoundGameobject != null)
                    {
                        UnityEditor.Undo.RegisterCompleteObjectUndo(listOfControlPointTriplets[i].backwardHelperPoint.connectionComponent_onBoundGameobject, nameOfUndoEntry);
                    }

                    if (listOfControlPointTriplets[i].anchorPoint.connectionComponent_onBoundGameobject != null)
                    {
                        UnityEditor.Undo.RegisterCompleteObjectUndo(listOfControlPointTriplets[i].anchorPoint.connectionComponent_onBoundGameobject, nameOfUndoEntry);
                    }

                    if (listOfControlPointTriplets[i].forwardHelperPoint.connectionComponent_onBoundGameobject != null)
                    {
                        UnityEditor.Undo.RegisterCompleteObjectUndo(listOfControlPointTriplets[i].forwardHelperPoint.connectionComponent_onBoundGameobject, nameOfUndoEntry);
                    }
                }
            }
#endif
        }

        public bool sheduledSceneViewRepaint_hasBeenExecuted = true;
        public void SheduleSceneViewRepaint()
        {
#if UNITY_EDITOR
            sheduledSceneViewRepaint_hasBeenExecuted = false;
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public void TryResheduleSceneViewRepaint()
        {
            //-> This function is necessary, because "EditorUtility.SetDirty(this)" does not reliably lead to scene view repaints
            //-> When in "OnInspectorGUI().*.DrawNonSerializedControlPointsList()" a "SheduleSceneViewRepaint()" is issued due to a changed inspector input value, it "mostly" works.
            //-> "mostly" means:
            //---> if the changed inspector field is e.g. a "Vector2" or "float" then it works
            //---> if the changed inspector field is a "enumPopup" then it doesn't work
            //-> This function repeats the "SetDirty" until the scene view repaint finally happens.

            if (sheduledSceneViewRepaint_hasBeenExecuted == false)
            {
                SheduleSceneViewRepaint();
            }
        }

        public Vector3 TransformPos_fromUnitsOfActiveDrawSpace_toGlobalSpace(Vector3 posToTransform_inUnitsOfActiveDrawSpace)
        {
            if (drawSpace == BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject)
            {
                return transform.TransformPoint(posToTransform_inUnitsOfActiveDrawSpace);
            }
            else
            {
                return posToTransform_inUnitsOfActiveDrawSpace;
            }
        }

        public Vector3 TransformPos_fromGlobalSpace_toUnitsOfActiveDrawSpace(Vector3 posToTransform_inUnitsOfGlobalSpace)
        {
            if (drawSpace == BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject)
            {
                return transform.InverseTransformPoint(posToTransform_inUnitsOfGlobalSpace);
            }
            else
            {
                return posToTransform_inUnitsOfGlobalSpace;
            }
        }

        public Vector3 TransformDirection_fromUnitsOfActiveDrawSpace_toGlobalSpace(Vector3 directionToTransform_inUnitsOfActiveDrawSpace)
        {
            if (drawSpace == BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject)
            {
                return transform.TransformDirection(directionToTransform_inUnitsOfActiveDrawSpace);
            }
            else
            {
                return directionToTransform_inUnitsOfActiveDrawSpace;
            }
        }

        public Vector3 TransformDirection_fromGlobalSpace_toUnitsOfActiveDrawSpace(Vector3 directionToTransform_inUnitsOfGlobalSpace)
        {
            if (drawSpace == BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject)
            {
                return transform.InverseTransformDirection(directionToTransform_inUnitsOfGlobalSpace);
            }
            else
            {
                return directionToTransform_inUnitsOfGlobalSpace;
            }
        }

        public float TransformLength_fromUnitsOfActiveDrawSpace_toGlobalSpace(float lengthToTransform_inUnitsOfActiveDrawSpace)
        {
            //This function only works correctly if all transforms of the parenting hierarchy have a homogeneous scale
            if (drawSpace == BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject)
            {
                return (lengthToTransform_inUnitsOfActiveDrawSpace * transform.lossyScale.x);
            }
            else
            {
                return lengthToTransform_inUnitsOfActiveDrawSpace;
            }
        }

        public float TransformLength_fromGlobalSpace_toUnitsOfActiveDrawSpace(float lengthToTransform_inUnitsOfGlobalSpace)
        {
            //This function only works correctly if all transforms of the parenting hierarchy have a homogeneous scale
            if ((drawSpace == BezierSplineDrawer.DrawSpace.localDefinedByThisGameobject) && (UtilitiesDXXL_Math.ApproximatelyZero(transform.lossyScale.x) == false))
            {
                return (lengthToTransform_inUnitsOfGlobalSpace / transform.lossyScale.x);
            }
            else
            {
                return lengthToTransform_inUnitsOfGlobalSpace;
            }
        }

    }

}
