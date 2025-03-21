import { cy } from "cypress"

// TODO: Need to check how it can work with react native
describe("Signup and verify 201 response", () => {
    it("Verify API", () => {
        const baseUrl = cy.env("DEVELOPMENT_URL")

        cy.intercept("*/signup").as("signup")

        cy.visit(`${baseUrl}/authentication/signup`)

        cy.wait("@signup").then(({ response }) => {
            expect(response.statusCode).to.eq(201)

            expect(response.body.success).to.be.true

            const userData = response.body.data
            expect(userData).to.have.property("email", "test2.test2@test.com")
            expect(userData).to.have.property("firstName", "test2")
            expect(userData).to.have.property("lastName", "string")
            expect(userData).to.have.property("username", "test2test2")
            expect(userData).to.have.property("phone", "123")
            expect(userData).to.have.property("type", "USER")
            expect(userData.avatarUrl).to.include("https://ui-avatars.com/api/")
        })
    })
})
