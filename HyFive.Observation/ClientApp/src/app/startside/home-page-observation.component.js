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
exports.HomePageForObservationComponent = void 0;
var core_1 = require("@angular/core");
var router_1 = require("@angular/router");
var urls_1 = require("../constants/urls");
var free_solid_svg_icons_1 = require("@fortawesome/free-solid-svg-icons");
var colors_1 = require("../utils/colors");
var SessionType_1 = require("../models/api/SessionType");
var HomePageForObservationComponent = function () {
    var _classDecorators = [(0, core_1.Component)({
            selector: "app-home-page-observation",
            templateUrl: "./home-page-observation.component.html",
        })];
    var _classDescriptor;
    var _classExtraInitializers = [];
    var _classThis;
    var HomePageForObservationComponent = _classThis = /** @class */ (function () {
        function HomePageForObservationComponent_1(router, fourIndicationsSessionService, handJewelrySessionService, gloveSessionService, institutionService, authorizationService) {
            this.router = router;
            this.fourIndicationsSessionService = fourIndicationsSessionService;
            this.handJewelrySessionService = handJewelrySessionService;
            this.gloveSessionService = gloveSessionService;
            this.institutionService = institutionService;
            this.authorizationService = authorizationService;
            this.SessionType = SessionType_1.SessionType;
            this.selectedDepartmentId = null;
            this.colors = colors_1.Colors;
            this.faCircle = free_solid_svg_icons_1.faCircle;
            this.faUserNurse = free_solid_svg_icons_1.faUserNurse;
            this.faCheck = free_solid_svg_icons_1.faCheck;
        }
        HomePageForObservationComponent_1.prototype.ngOnInit = function () {
            var _this = this;
            this.resetState();
            this.router.events.subscribe(function (e) {
                if (e instanceof router_1.NavigationEnd) {
                    _this.resetState();
                }
            });
        };
        HomePageForObservationComponent_1.prototype.resetState = function () {
            var _this = this;
            this.selectedSessionType = SessionType_1.SessionType.NotSelected;
            this.timekeeping = false;
            this.gloveUse = false;
            this.roleSelected = [];
            this.selectedDepartmentId = null;
            this.showHomePage = true;
            this.showProtectiveEquipment = false;
            this.institutionOptions = [];
            this.selectedInstitutionOptionId = 0;
            this.institution = null;
            this.institutionService
                .getInstitutions()
                .subscribe(function (institutions) {
                var _a;
                _this.institutionOptions = institutions;
                var enesteInstitusjon = null;
                if (((_a = _this.institutionOptions) === null || _a === void 0 ? void 0 : _a.length) == 1) {
                    enesteInstitusjon = _this.institutionOptions[0];
                }
                _this.institutionService
                    .getSelectedInstitution()
                    .subscribe(function (selectedInstitution) {
                    if (selectedInstitution) {
                        _this.institution = selectedInstitution;
                    }
                    if (!selectedInstitution && enesteInstitusjon) {
                        _this.institution = enesteInstitusjon;
                        _this.institutionService.updateSelectedInstitutionId(enesteInstitusjon.id);
                    }
                    _this.selectedInstitutionOptionId = _this.institution
                        ? _this.institution.id
                        : 0;
                });
            });
            this.authorizationService.getUser().subscribe(function (user) {
                _this.user = user;
            });
        };
        HomePageForObservationComponent_1.prototype.startObservation = function () {
            if (!this.selectedDepartmentId) {
                alert("Select a department");
                return;
            }
            if (!this.roleSelected.filter(function (r) { return r.isSelected; }).length) {
                alert("Select one or more roles");
                return;
            }
            switch (this.selectedSessionType) {
                case SessionType_1.SessionType.NotSelected:
                    alert("Select the sessionType you want to start");
                    break;
                case SessionType_1.SessionType.FourIndications:
                    this.startFourIndicationsSession();
                    break;
                case SessionType_1.SessionType.HandJewelry:
                    this.startHandJewelrySession();
                    break;
                case SessionType_1.SessionType.Gloves:
                    this.startGloveSession();
                    break;
                case SessionType_1.SessionType.ProtectiveEquipment:
                    this.showHomePage = false;
                    this.showProtectiveEquipment = true;
                    break;
                default:
                    alert("Observation of ".concat(Object.values(SessionType_1.SessionType)[this.selectedSessionType], " is not supported yet"));
                    break;
            }
        };
        HomePageForObservationComponent_1.prototype.startFourIndicationsSession = function () {
            var sessionId = this.fourIndicationsSessionService.createSessionView(this.gloveUse, this.timekeeping, this.roleSelected.filter(function (r) { return r.isSelected; }).map(function (r) { return r.role; }), this.getSelectedDepartment());
            this.router.navigate([urls_1.Urls.RegisterFourndicationsUrl], {
                queryParams: { sessionId: sessionId },
            });
        };
        HomePageForObservationComponent_1.prototype.startHandJewelrySession = function () {
            var sessionId = this.handJewelrySessionService.createSessionView(this.roleSelected.filter(function (r) { return r.isSelected; }).map(function (r) { return r.role; }), this.getSelectedDepartment());
            this.router.navigate([urls_1.Urls.RegisterHandJewelryUrl], {
                queryParams: { sessionId: sessionId },
            });
        };
        HomePageForObservationComponent_1.prototype.startGloveSession = function () {
            var sessionId = this.gloveSessionService.createSessionView(this.gloveUse, this.roleSelected.filter(function (r) { return r.isSelected; }).map(function (r) { return r.role; }), this.getSelectedDepartment());
            this.router.navigate([urls_1.Urls.RegisterGloveUrl], {
                queryParams: { sessionId: sessionId },
            });
        };
        HomePageForObservationComponent_1.prototype.selectedInstitutionChanged = function () {
            var _this = this;
            // change institution
            this.institutionService.updateSelectedInstitutionId(this.selectedInstitutionOptionId);
            // change selectedInstitution
            this.institution = this.institutionOptions.find(function (x) { return x.id === _this.selectedInstitutionOptionId; });
            this.selectedDepartmentId = null;
            this.selectedDepartmentChanged();
        };
        HomePageForObservationComponent_1.prototype.getSelectedDepartment = function () {
            var _this = this;
            return this.institution.departments.find(function (x) { return x.id === parseInt(_this.selectedDepartmentId); });
        };
        HomePageForObservationComponent_1.prototype.selectedDepartmentChanged = function () {
            var _this = this;
            var _a;
            this.roleSelected = (_a = this.institution.departments
                .find(function (x) { return x.id === parseInt(_this.selectedDepartmentId); })) === null || _a === void 0 ? void 0 : _a.roles.map(function (role) {
                return { role: role, isSelected: false };
            });
        };
        HomePageForObservationComponent_1.prototype.canNotStartObservation = function () {
            return (this.selectedDepartmentId === null ||
                this.roleSelected.filter(function (r) { return r.isSelected; }).length === 0 ||
                this.selectedSessionType === SessionType_1.SessionType.NotSelected);
        };
        return HomePageForObservationComponent_1;
    }());
    __setFunctionName(_classThis, "HomePageForObservationComponent");
    (function () {
        var _metadata = typeof Symbol === "function" && Symbol.metadata ? Object.create(null) : void 0;
        __esDecorate(null, _classDescriptor = { value: _classThis }, _classDecorators, { kind: "class", name: _classThis.name, metadata: _metadata }, null, _classExtraInitializers);
        HomePageForObservationComponent = _classThis = _classDescriptor.value;
        if (_metadata) Object.defineProperty(_classThis, Symbol.metadata, { enumerable: true, configurable: true, writable: true, value: _metadata });
        __runInitializers(_classThis, _classExtraInitializers);
    })();
    return HomePageForObservationComponent = _classThis;
}();
exports.HomePageForObservationComponent = HomePageForObservationComponent;
//# sourceMappingURL=home-page-observation.component.js.map