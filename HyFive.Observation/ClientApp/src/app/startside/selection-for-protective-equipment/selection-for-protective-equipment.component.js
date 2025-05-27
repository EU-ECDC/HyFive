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
exports.SelectionForProtectiveEquipmentComponent = void 0;
var core_1 = require("@angular/core");
var urls_1 = require("../../constants/urls");
var ProtectiveEquipment_setting_mapper_1 = require("../../utils/ProtectiveEquipment-setting-mapper");
var free_solid_svg_icons_1 = require("@fortawesome/free-solid-svg-icons");
var ProtectiveEquipmentTypeConstants_1 = require("../../models/api/ProtectiveEquipmentTypeConstants");
var SelectionForProtectiveEquipmentComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: 'app-selection-for-protective-equipment',
            templateUrl: './selection-for-protective-equipment.component.html'
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var _sessionView_decorators;
    var _sessionView_initializers = [];
    var _sessionView_extraInitializers = [];
    var _roles_decorators;
    var _roles_initializers = [];
    var _roles_extraInitializers = [];
    var _department_decorators;
    var _department_initializers = [];
    var _department_extraInitializers = [];
    var _settingEquipmentWasChanged_decorators;
    var _settingEquipmentWasChanged_initializers = [];
    var _settingEquipmentWasChanged_extraInitializers = [];
    var SelectionForProtectiveEquipmentComponent = _classThis = /** @class */ (function () {
        function SelectionForProtectiveEquipmentComponent_1(protectiveEquipmentCodingService, protectiveEquipmentSessionService, router) {
            this.protectiveEquipmentCodingService = protectiveEquipmentCodingService;
            this.protectiveEquipmentSessionService = protectiveEquipmentSessionService;
            this.router = router;
            this.hasPredefinedEquipment = false;
            this.faCircle = free_solid_svg_icons_1.faCircle;
            this.protectiveEquipmentSettingMapper = ProtectiveEquipment_setting_mapper_1.ProtectiveEquipmentSettingMapper;
            this.sessionView = __runInitializers(this, _sessionView_initializers, null);
            this.roles = (__runInitializers(this, _sessionView_extraInitializers), __runInitializers(this, _roles_initializers, void 0));
            this.department = (__runInitializers(this, _roles_extraInitializers), __runInitializers(this, _department_initializers, void 0));
            this.settingEquipmentWasChanged = (__runInitializers(this, _department_extraInitializers), __runInitializers(this, _settingEquipmentWasChanged_initializers, new core_1.EventEmitter()));
            __runInitializers(this, _settingEquipmentWasChanged_extraInitializers);
            this.protectiveEquipmentCodingService = protectiveEquipmentCodingService;
            this.protectiveEquipmentSessionService = protectiveEquipmentSessionService;
            this.router = router;
        }
        SelectionForProtectiveEquipmentComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            this.protectiveEquipmentCodingService.getProtectiveEquipmentSettings().subscribe(function (settings) {
                _this.settings = settings;
            });
        };
        SelectionForProtectiveEquipmentComponent_1.prototype.changeSelectedSetting = function (setting) {
            var _a, _b, _c;
            setting.equipmentTypes = this.showEquipmentByOrder(setting.equipmentTypes);
            setting.equipmentTypes = setting.equipmentTypes.map(function (u) { u.isRequired = u.isDefault; return u; });
            this.selectedSetting = setting;
            this.hasPredefinedEquipment = ((_c = (_b = (_a = this.selectedSetting) === null || _a === void 0 ? void 0 : _a.equipmentTypes) === null || _b === void 0 ? void 0 : _b.filter(function (u) { return u.isRequired; })) === null || _c === void 0 ? void 0 : _c.length) > 0;
        };
        SelectionForProtectiveEquipmentComponent_1.prototype.startObservation = function () {
            var selectedRoles = this.roles.filter(function (roleSelected) { return roleSelected.isSelected; }).map(function (roleSelected) { return roleSelected.role; });
            var sessionId = this.protectiveEquipmentSessionService.createSessionView(selectedRoles, this.department, this.selectedSetting);
            this.router.navigate([urls_1.Urls.RegisterProtectiveEquipmentUrl], { queryParams: { sessionId: sessionId } });
        };
        SelectionForProtectiveEquipmentComponent_1.prototype.changeSettingsAndEquipment = function () {
            var _this = this;
            this.sessionView.setting = this.selectedSetting;
            this.sessionView.card = this.sessionView.card.map(function (k) { k.equipment = _this.selectedSetting.equipmentTypes; return k; });
            this.settingEquipmentWasChanged.emit(this.sessionView);
        };
        SelectionForProtectiveEquipmentComponent_1.prototype.showEquipmentByOrder = function (protectiveEquipmentTypes) {
            var protectiveEquipmentTypesByOrder = [];
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.Gloves; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.Gloves; })[0]);
            }
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.PlasticApron; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.PlasticApron; })[0]);
            }
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.CareGown; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.CareGown; })[0]);
            }
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.InfectionGown; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.InfectionGown; })[0]);
            }
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.FaceMask; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.FaceMask; })[0]);
            }
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.RespiratoryProtection; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.RespiratoryProtection; })[0]);
            }
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.EyeProtection; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.EyeProtection; })[0]);
            }
            if (protectiveEquipmentTypes.find(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.Hood; })) {
                protectiveEquipmentTypesByOrder.push(protectiveEquipmentTypes.filter(function (b) { return b.code === ProtectiveEquipmentTypeConstants_1.ProtectiveEquipmentTypeConstants.Hood; })[0]);
            }
            return protectiveEquipmentTypesByOrder;
        };
        return SelectionForProtectiveEquipmentComponent_1;
    }());
    __setFunctionName(_classThis, "SelectionForProtectiveEquipmentComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        _sessionView_decorators = [(0, core_1.Input)("sessionView")];
        _roles_decorators = [(0, core_1.Input)("roles")];
        _department_decorators = [(0, core_1.Input)("department")];
        _settingEquipmentWasChanged_decorators = [(0, core_1.Output)("settingEquipmentWasChanged")];
        __esDecorate(null, null, _sessionView_decorators, { kind: "field", name: "sessionView", static: false, private: false, access: { has: function (obj) { return "sessionView" in obj; }, get: function (obj) { return obj.sessionView; }, set: function (obj, value) { obj.sessionView = value; } }, metadata: _metadata }, _sessionView_initializers, _sessionView_extraInitializers);
        __esDecorate(null, null, _roles_decorators, { kind: "field", name: "roles", static: false, private: false, access: { has: function (obj) { return "roles" in obj; }, get: function (obj) { return obj.roles; }, set: function (obj, value) { obj.roles = value; } }, metadata: _metadata }, _roles_initializers, _roles_extraInitializers);
        __esDecorate(null, null, _department_decorators, { kind: "field", name: "department", static: false, private: false, access: { has: function (obj) { return "department" in obj; }, get: function (obj) { return obj.department; }, set: function (obj, value) { obj.department = value; } }, metadata: _metadata }, _department_initializers, _department_extraInitializers);
        __esDecorate(null, null, _settingEquipmentWasChanged_decorators, { kind: "field", name: "settingEquipmentWasChanged", static: false, private: false, access: { has: function (obj) { return "settingEquipmentWasChanged" in obj; }, get: function (obj) { return obj.settingEquipmentWasChanged; }, set: function (obj, value) { obj.settingEquipmentWasChanged = value; } }, metadata: _metadata }, _settingEquipmentWasChanged_initializers, _settingEquipmentWasChanged_extraInitializers);
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        SelectionForProtectiveEquipmentComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return SelectionForProtectiveEquipmentComponent = _classThis;
}();
exports.SelectionForProtectiveEquipmentComponent = SelectionForProtectiveEquipmentComponent;
//# sourceMappingURL=selection-for-protective-equipment.component.js.map