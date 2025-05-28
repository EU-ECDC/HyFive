"use strict";
var __esDecorate = (this && this.__esDecorate) || function (ctor, descriptorIn, decorators, contextIn, initializers, extraInitializers) {
    function accept(f) { if (f !== void 0 && typeof f !== "function") throw new TypeError("Function expected"); return f; }
    var kind = contextIn.kind, key = kind === "getter" ? "get" : kind === "setter" ? "set" : "value";
    var target = !descriptorIn && ctor ? contextIn["static"] ? ctor : ctor.prototype : null;
    var descriptor = descriptorIn || (target ? Object.getOwnPropertyDescriptor(target, contextIn.name) : {});
    var _, done = false;
    for (var i = decorators.length - 1; i >= 0; i--) {
        var context = {};
        for (var p in contextIn) context[p] = p === "access" ? {} : contextIn[p];
        for (var p in contextIn.access) context.access[p] = contextIn.access[p];
        context.addInitializer = function (f) { if (done) throw new TypeError("Cannot add initializers after decoration has completed"); extraInitializers.push(accept(f || null)); };
        var result = (0, decorators[i])(kind === "accessor" ? { get: descriptor.get, set: descriptor.set } : descriptor[key], context);
        if (kind === "accessor") {
            if (result === void 0) continue;
            if (result === null || typeof result !== "object") throw new TypeError("Object expected");
            if (_ = accept(result.get)) descriptor.get = _;
            if (_ = accept(result.set)) descriptor.set = _;
            if (_ = accept(result.init)) initializers.unshift(_);
        }
        else if (_ = accept(result)) {
            if (kind === "field") initializers.unshift(_);
            else descriptor[key] = _;
        }
    }
    if (target) Object.defineProperty(target, contextIn.name, descriptor);
    done = true;
};
var __runInitializers = (this && this.__runInitializers) || function (thisArg, initializers, value) {
    var useValue = arguments.length > 2;
    for (var i = 0; i < initializers.length; i++) {
        value = useValue ? initializers[i].call(thisArg, value) : initializers[i].call(thisArg);
    }
    return useValue ? value : void 0;
};
var __setFunctionName = (this && this.__setFunctionName) || function (f, name, prefix) {
    if (typeof name === "symbol") name = name.description ? "[".concat(name.description, "]") : "";
    return Object.defineProperty(f, "name", { configurable: true, value: prefix ? "".concat(prefix, " ", name) : name });
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.EditFourIndicationsObservationComponent = void 0;
var core_1 = require("@angular/core");
var free_solid_svg_icons_1 = require("@fortawesome/free-solid-svg-icons");
var colors_1 = require("../../utils/colors");
var dialogueTexts_1 = require("../../constants/dialogueTexts");
var ActivityTypeConstants_1 = require("../../models/api/ActivityTypeConstants");
var ActivityTypeNotExecutedMapper_1 = require("../../utils/ActivityTypeNotExecutedMapper");
var uuid_1 = require("../../utils/uuid");
var ActivityTypeNotExecutedId_1 = require("../../models/api/ActivityTypeNotExecutedId");
var Activities_1 = require("../../constants/Activities");
var EditFourIndicationsObservationComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-edit-four-indications-observation',
            templateUrl: './edit-four-indications-observation.component.html'
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var _isReadonly_decorators;
    var _isReadonly_initializers = [];
    var _isReadonly_extraInitializers = [];
    var _observation_decorators;
    var _observation_initializers = [];
    var _observation_extraInitializers = [];
    var _department_decorators;
    var _department_initializers = [];
    var _department_extraInitializers = [];
    var _gloveUseMustBeRegistered_decorators;
    var _gloveUseMustBeRegistered_initializers = [];
    var _gloveUseMustBeRegistered_extraInitializers = [];
    var _timeShouldBeRegistred_decorators;
    var _timeShouldBeRegistred_initializers = [];
    var _timeShouldBeRegistred_extraInitializers = [];
    var _observationDeletedEvent_decorators;
    var _observationDeletedEvent_initializers = [];
    var _observationDeletedEvent_extraInitializers = [];
    var EditFourIndicationsObservationComponent = _classThis = /** @class */ (function () {
        function EditFourIndicationsObservationComponent_1(sessionService, activityService) {
            this.sessionService = sessionService;
            this.activityService = activityService;
            this.isEditMode = false;
            this.ActivityTypeConstants = ActivityTypeConstants_1.ActivityTypeConstants;
            this.Colors = colors_1.Colors;
            this.dialogueTexts = dialogueTexts_1.DialogueTexts;
            this.id = uuid_1.Uuid.generateUUID().substr(4);
            this.showActivityTypeNotExecuted = false;
            this.alcohol = Activities_1.Activities.Alcohol;
            this.wash = Activities_1.Activities.Wash;
            this.faHandHoldingWater = free_solid_svg_icons_1.faHandHoldingWater;
            this.faHandsWash = free_solid_svg_icons_1.faHandsWash;
            this.faSave = free_solid_svg_icons_1.faSave;
            this.faTrashAlt = free_solid_svg_icons_1.faTrashAlt;
            this.faTimesCircle = free_solid_svg_icons_1.faTimesCircle;
            this.isReadonly = __runInitializers(this, _isReadonly_initializers, false);
            this.observation = (__runInitializers(this, _isReadonly_extraInitializers), __runInitializers(this, _observation_initializers, void 0));
            this.department = (__runInitializers(this, _observation_extraInitializers), __runInitializers(this, _department_initializers, void 0));
            this.gloveUseMustBeRegistered = (__runInitializers(this, _department_extraInitializers), __runInitializers(this, _gloveUseMustBeRegistered_initializers, void 0));
            this.timeShouldBeRegistred = (__runInitializers(this, _gloveUseMustBeRegistered_extraInitializers), __runInitializers(this, _timeShouldBeRegistred_initializers, void 0));
            this.observationDeletedEvent = (__runInitializers(this, _timeShouldBeRegistred_extraInitializers), __runInitializers(this, _observationDeletedEvent_initializers, new core_1.EventEmitter()));
            __runInitializers(this, _observationDeletedEvent_extraInitializers);
            this.sessionService = sessionService;
            this.activityService = activityService;
            this.activityTypeNotExecutedSelection = ActivityTypeNotExecutedMapper_1.ActivityTypeNotExecutedMapper.getNameMap();
        }
        EditFourIndicationsObservationComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            if (this.observation.activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                this.showActivityTypeNotExecuted = true;
                this.selectedActivityTypeNotExecutedSelectionId = this.getSelectedActivityTypeNotExecutedId(this.observation.activity);
            }
            this.activityService.getActivityTypes().subscribe(function (activityTypes) {
                _this.activityTypes = activityTypes;
            });
            if (this.isReadonly && this.observation.activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                if (this.observation.activity.gloveUsed === null)
                    this.gloveUseText = this.activityTypeNotExecutedSelection[0].name;
                else if (this.observation.activity.gloveUsed === true)
                    this.gloveUseText = this.activityTypeNotExecutedSelection[1].name;
                else if (this.observation.activity.gloveUsed === false)
                    this.gloveUseText = this.activityTypeNotExecutedSelection[2].name;
            }
        };
        EditFourIndicationsObservationComponent_1.prototype.getSelectedActivityTypeNotExecutedId = function (activity) {
            if (activity.gloveUsed === null && activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                return ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.NotExecuted.toString();
            }
            else if (activity.gloveUsed === true && activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                return ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.GloveWasUsed.toString();
            }
            else if (activity.gloveUsed === false && activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                return ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.GloveWasNotUsed.toString();
            }
        };
        EditFourIndicationsObservationComponent_1.prototype.registerActivity = function (activity) {
            var _a;
            if (this.observation.activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                this.notExecutedActivity = this.observation.activity;
            }
            if (activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                this.showActivityTypeNotExecuted = true;
                if (this.notExecutedActivity) {
                    this.selectedActivityTypeNotExecutedSelectionId = this.selectedActivityTypeNotExecuted((_a = this.notExecutedActivity) === null || _a === void 0 ? void 0 : _a.gloveUsed);
                }
                else {
                    this.selectedActivityTypeNotExecutedSelectionId = ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.NotExecuted.toString();
                }
                activity.timeSpent = this.observation.activity.timeSpent;
                this.observation.activity = !this.notExecutedActivity ? activity : this.notExecutedActivity;
                this.notExecutedActivity = null;
            }
            else {
                this.showActivityTypeNotExecuted = false;
                activity.timeSpent = !this.observation.activity.timeSpent ? 0 : this.observation.activity.timeSpent;
                activity.timeRecordingWasDone = activity.timeSpent > 0;
                this.observation.activity = activity;
                if (this.observation.activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.Handwash) {
                    this.wash = '';
                    this.alcohol = Activities_1.Activities.Alcohol;
                }
                else {
                    this.alcohol = '';
                    this.wash = Activities_1.Activities.Wash;
                }
            }
        };
        EditFourIndicationsObservationComponent_1.prototype.selectedActivityTypeNotExecuted = function (gloveUsed) {
            if (gloveUsed === true) {
                return ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.GloveWasUsed.toString();
            }
            else if (gloveUsed === false) {
                return ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.GloveWasNotUsed.toString();
            }
            else if (gloveUsed || gloveUsed === null) {
                return ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.NotExecuted.toString();
            }
        };
        EditFourIndicationsObservationComponent_1.prototype.indicationSelectionChanged = function (selectedIndications) {
            this.observation.indicationTypes = selectedIndications;
        };
        EditFourIndicationsObservationComponent_1.prototype.saveObservation = function () {
            if (this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                this.observation = this.registerActivityTypeNotExecuted(this.observation);
            }
            else if (!this.gloveUseMustBeRegistered && this.observation.activity.activityType.code === ActivityTypeConstants_1.ActivityTypeConstants.NotExecuted) {
                this.observation.activity.timeSpent = 0;
            }
            this.sessionService.changeObservation(this.observation);
            this.isEditMode = false;
            this.notExecutedActivity = null;
        };
        EditFourIndicationsObservationComponent_1.prototype.deleteObservation = function () {
            this.sessionService.deleteObservation(this.observation);
            this.observationDeletedEvent.emit();
        };
        EditFourIndicationsObservationComponent_1.prototype.isActivitySelected = function (activityTypeCode) {
            return this.observation.activity.activityType.code === activityTypeCode;
        };
        EditFourIndicationsObservationComponent_1.prototype.getActivityType = function (code) {
            var _a;
            return (_a = this.activityTypes) === null || _a === void 0 ? void 0 : _a.find(function (x) { return x.code === code; });
        };
        EditFourIndicationsObservationComponent_1.prototype.selectedActivityTypeNotExecutedChanged = function (selectedId) {
            this.selectedActivityTypeNotExecutedSelectionId = selectedId;
        };
        EditFourIndicationsObservationComponent_1.prototype.registerActivityTypeNotExecuted = function (observation) {
            if ((!this.selectedActivityTypeNotExecutedSelectionId || this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.NotExecuted.toString())) {
                observation.activity.gloveUsed = null;
            }
            else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.GloveWasUsed.toString()) {
                observation.activity.gloveUsed = true;
            }
            else if (this.selectedActivityTypeNotExecutedSelectionId === ActivityTypeNotExecutedId_1.ActivityTypeNotExecutedId.GloveWasNotUsed.toString()) {
                observation.activity.gloveUsed = false;
            }
            observation.activity.timeRecordingWasDone = false;
            observation.activity.timeSpent = 0;
            return observation;
        };
        EditFourIndicationsObservationComponent_1.prototype.registerComment = function (comment) {
            this.observation.comment = comment;
        };
        return EditFourIndicationsObservationComponent_1;
    }());
    __setFunctionName(_classThis, "EditFourIndicationsObservationComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        _isReadonly_decorators = [(0, core_1.Input)("isReadonly")];
        _observation_decorators = [(0, core_1.Input)("observation")];
        _department_decorators = [(0, core_1.Input)("department")];
        _gloveUseMustBeRegistered_decorators = [(0, core_1.Input)("gloveUseMustBeRegistered")];
        _timeShouldBeRegistred_decorators = [(0, core_1.Input)("timeShouldBeRegistred")];
        _observationDeletedEvent_decorators = [(0, core_1.Output)("observationDeletedEvent")];
        __esDecorate(null, null, _isReadonly_decorators, { kind: "field", name: "isReadonly", static: false, private: false, access: { has: function (obj) { return "isReadonly" in obj; }, get: function (obj) { return obj.isReadonly; }, set: function (obj, value) { obj.isReadonly = value; } }, metadata: _metadata }, _isReadonly_initializers, _isReadonly_extraInitializers);
        __esDecorate(null, null, _observation_decorators, { kind: "field", name: "observation", static: false, private: false, access: { has: function (obj) { return "observation" in obj; }, get: function (obj) { return obj.observation; }, set: function (obj, value) { obj.observation = value; } }, metadata: _metadata }, _observation_initializers, _observation_extraInitializers);
        __esDecorate(null, null, _department_decorators, { kind: "field", name: "department", static: false, private: false, access: { has: function (obj) { return "department" in obj; }, get: function (obj) { return obj.department; }, set: function (obj, value) { obj.department = value; } }, metadata: _metadata }, _department_initializers, _department_extraInitializers);
        __esDecorate(null, null, _gloveUseMustBeRegistered_decorators, { kind: "field", name: "gloveUseMustBeRegistered", static: false, private: false, access: { has: function (obj) { return "gloveUseMustBeRegistered" in obj; }, get: function (obj) { return obj.gloveUseMustBeRegistered; }, set: function (obj, value) { obj.gloveUseMustBeRegistered = value; } }, metadata: _metadata }, _gloveUseMustBeRegistered_initializers, _gloveUseMustBeRegistered_extraInitializers);
        __esDecorate(null, null, _timeShouldBeRegistred_decorators, { kind: "field", name: "timeShouldBeRegistred", static: false, private: false, access: { has: function (obj) { return "timeShouldBeRegistred" in obj; }, get: function (obj) { return obj.timeShouldBeRegistred; }, set: function (obj, value) { obj.timeShouldBeRegistred = value; } }, metadata: _metadata }, _timeShouldBeRegistred_initializers, _timeShouldBeRegistred_extraInitializers);
        __esDecorate(null, null, _observationDeletedEvent_decorators, { kind: "field", name: "observationDeletedEvent", static: false, private: false, access: { has: function (obj) { return "observationDeletedEvent" in obj; }, get: function (obj) { return obj.observationDeletedEvent; }, set: function (obj, value) { obj.observationDeletedEvent = value; } }, metadata: _metadata }, _observationDeletedEvent_initializers, _observationDeletedEvent_extraInitializers);
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        EditFourIndicationsObservationComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return EditFourIndicationsObservationComponent = _classThis;
}();
exports.EditFourIndicationsObservationComponent = EditFourIndicationsObservationComponent;
//# sourceMappingURL=edit-four-indications-observation.component.js.map