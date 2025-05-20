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
exports.EditHandjewelryObservationsComponent = void 0;
var core_1 = require("@angular/core");
var EditHandjewelryObservationsComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-edit-handjewelry-observations',
            templateUrl: './edit-handjewelry-observations.component.html'
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var _observations_decorators;
    var _observations_initializers = [];
    var _observations_extraInitializers = [];
    var _sessionId_decorators;
    var _sessionId_initializers = [];
    var _sessionId_extraInitializers = [];
    var _department_decorators;
    var _department_initializers = [];
    var _department_extraInitializers = [];
    var _canEdit_decorators;
    var _canEdit_initializers = [];
    var _canEdit_extraInitializers = [];
    var _observationUpdatedEvent_decorators;
    var _observationUpdatedEvent_initializers = [];
    var _observationUpdatedEvent_extraInitializers = [];
    var _observationDeletedEvent_decorators;
    var _observationDeletedEvent_initializers = [];
    var _observationDeletedEvent_extraInitializers = [];
    var EditHandjewelryObservationsComponent = _classThis = /** @class */ (function () {
        function EditHandjewelryObservationsComponent_1(observationService, toastrService, keyEventService, handJewelryTypeService) {
            this.observationService = observationService;
            this.toastrService = toastrService;
            this.keyEventService = keyEventService;
            this.handJewelryTypeService = handJewelryTypeService;
            this.observations = __runInitializers(this, _observations_initializers, void 0);
            this.sessionId = (__runInitializers(this, _observations_extraInitializers), __runInitializers(this, _sessionId_initializers, void 0));
            this.department = (__runInitializers(this, _sessionId_extraInitializers), __runInitializers(this, _department_initializers, void 0));
            this.canEdit = (__runInitializers(this, _department_extraInitializers), __runInitializers(this, _canEdit_initializers, false));
            this.observationUpdatedEvent = (__runInitializers(this, _canEdit_extraInitializers), __runInitializers(this, _observationUpdatedEvent_initializers, new core_1.EventEmitter()));
            this.observationDeletedEvent = (__runInitializers(this, _observationUpdatedEvent_extraInitializers), __runInitializers(this, _observationDeletedEvent_initializers, new core_1.EventEmitter()));
            this.handjewelryObservationAsChanged = __runInitializers(this, _observationDeletedEvent_extraInitializers);
            this.handJewelrySelection = [];
            this.handJewelryTypes = [];
        }
        EditHandjewelryObservationsComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            this.handJewelryTypeService.getHandJewelryTypes().subscribe(function (types) {
                _this.handJewelryTypes = types;
            });
            this.keyEventService.escapeKeyEvent.subscribe(function (event) {
                if (_this.handjewelryObservationAsChanged)
                    _this.handjewelryObservationAsChanged = null;
            });
        };
        EditHandjewelryObservationsComponent_1.prototype.selectObservation = function (observation) {
            if (!this.canEdit) {
                return;
            }
            this.handJewelrySelection = this.handJewelryTypes.map(function (t) {
                return {
                    type: t.code,
                    disabled: false,
                    isSelected: observation.handJewelryTypes.map(function (ht) { return ht.code; }).indexOf(t.code) !== -1,
                    name: t.name
                };
            });
            this.handjewelryObservationAsChanged = {
                id: observation.id,
                sessionId: this.sessionId,
                handJewelries: observation.handJewelryTypes,
                comment: observation.comment,
                role: observation.role,
                registrationTime: observation.registeredTime
            };
        };
        EditHandjewelryObservationsComponent_1.prototype.changeComment = function (comment) {
            this.handjewelryObservationAsChanged.comment = comment;
        };
        EditHandjewelryObservationsComponent_1.prototype.updateHandJewelryObservation = function () {
            var _this = this;
            var types = this.handJewelrySelection.filter(function (h) { return h.isSelected; }).map(function (hsv) { return hsv.type; });
            this.handjewelryObservationAsChanged.handJewelries = this.handJewelryTypes.filter(function (h) { return types.indexOf(h.code) !== -1; });
            this.observationService.updateHandJewelryObservation(this.handjewelryObservationAsChanged).subscribe(function (isUpdated) {
                _this.handjewelryObservationAsChanged = null;
                _this.toastrService.success('The observation was updated');
                _this.observationUpdatedEvent.emit();
            }, function (error) {
                _this.toastrService.error((error === null || error === void 0 ? void 0 : error.error) ? error.error : error, 'Error updating observation: ', { disableTimeOut: true });
            });
        };
        EditHandjewelryObservationsComponent_1.prototype.deleteHandJewelryObservation = function () {
            var _this = this;
            this.observationService.deleteHandJewelryObservation(this.handjewelryObservationAsChanged.id, this.sessionId).subscribe(function () {
                _this.handjewelryObservationAsChanged = null;
                _this.toastrService.success('The observation was deleted');
                _this.observationDeletedEvent.emit();
            }, function (error) {
                _this.toastrService.error((error === null || error === void 0 ? void 0 : error.error) ? error.error : error, 'Error deleting observation', { disableTimeOut: true });
            });
        };
        EditHandjewelryObservationsComponent_1.prototype.cancelEditObservation = function (event) {
            event.stopPropagation();
            this.handjewelryObservationAsChanged = null;
        };
        EditHandjewelryObservationsComponent_1.prototype.selectRole = function (role) {
            this.handjewelryObservationAsChanged.role = role;
        };
        return EditHandjewelryObservationsComponent_1;
    }());
    __setFunctionName(_classThis, "EditHandjewelryObservationsComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        _observations_decorators = [(0, core_1.Input)()];
        _sessionId_decorators = [(0, core_1.Input)()];
        _department_decorators = [(0, core_1.Input)()];
        _canEdit_decorators = [(0, core_1.Input)()];
        _observationUpdatedEvent_decorators = [(0, core_1.Output)()];
        _observationDeletedEvent_decorators = [(0, core_1.Output)()];
        __esDecorate(null, null, _observations_decorators, { kind: "field", name: "observations", static: false, private: false, access: { has: function (obj) { return "observations" in obj; }, get: function (obj) { return obj.observations; }, set: function (obj, value) { obj.observations = value; } }, metadata: _metadata }, _observations_initializers, _observations_extraInitializers);
        __esDecorate(null, null, _sessionId_decorators, { kind: "field", name: "sessionId", static: false, private: false, access: { has: function (obj) { return "sessionId" in obj; }, get: function (obj) { return obj.sessionId; }, set: function (obj, value) { obj.sessionId = value; } }, metadata: _metadata }, _sessionId_initializers, _sessionId_extraInitializers);
        __esDecorate(null, null, _department_decorators, { kind: "field", name: "department", static: false, private: false, access: { has: function (obj) { return "department" in obj; }, get: function (obj) { return obj.department; }, set: function (obj, value) { obj.department = value; } }, metadata: _metadata }, _department_initializers, _department_extraInitializers);
        __esDecorate(null, null, _canEdit_decorators, { kind: "field", name: "canEdit", static: false, private: false, access: { has: function (obj) { return "canEdit" in obj; }, get: function (obj) { return obj.canEdit; }, set: function (obj, value) { obj.canEdit = value; } }, metadata: _metadata }, _canEdit_initializers, _canEdit_extraInitializers);
        __esDecorate(null, null, _observationUpdatedEvent_decorators, { kind: "field", name: "observationUpdatedEvent", static: false, private: false, access: { has: function (obj) { return "observationUpdatedEvent" in obj; }, get: function (obj) { return obj.observationUpdatedEvent; }, set: function (obj, value) { obj.observationUpdatedEvent = value; } }, metadata: _metadata }, _observationUpdatedEvent_initializers, _observationUpdatedEvent_extraInitializers);
        __esDecorate(null, null, _observationDeletedEvent_decorators, { kind: "field", name: "observationDeletedEvent", static: false, private: false, access: { has: function (obj) { return "observationDeletedEvent" in obj; }, get: function (obj) { return obj.observationDeletedEvent; }, set: function (obj, value) { obj.observationDeletedEvent = value; } }, metadata: _metadata }, _observationDeletedEvent_initializers, _observationDeletedEvent_extraInitializers);
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        EditHandjewelryObservationsComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return EditHandjewelryObservationsComponent = _classThis;
}();
exports.EditHandjewelryObservationsComponent = EditHandjewelryObservationsComponent;
//# sourceMappingURL=edit-handjewelry-observations.component.js.map